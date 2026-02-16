using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrewAI.DotNet.Core.Interfaces;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace CrewAI.DotNet.Core.Models
{
    public class Agent : IAgent
    {
        public string Role { get; set; }
        public string Goal { get; set; }
        public string Backstory { get; set; }
        public IList<KernelPlugin> Tools { get; set; } = new List<KernelPlugin>();

        public Kernel? Kernel { get; set; }

        public Agent(string role, string goal, string backstory, Kernel? kernel = null)
        {
            Role = role;
            Goal = goal;
            Backstory = backstory;
            Kernel = kernel;
        }

        public async Task<string> ExecuteAsync(ICrewTask task, IMemoryContext? memoryContext = null)
        {
            if (Kernel == null)
            {
                throw new InvalidOperationException("Agent Kernel is not initialized.");
            }

            // Create a scoped kernel to avoid modifying the shared kernel
            // Assuming Kernel.Clone() is available in this version.
            var scopedKernel = Kernel.Clone();

            // Add tools to the scoped kernel if not already present
            foreach (var tool in Tools)
            {
                if (!scopedKernel.Plugins.Contains(tool.Name))
                {
                    scopedKernel.Plugins.Add(tool);
                }
            }

            var context = await BuildContextAsync(task, memoryContext);

            var prompt = $@"
You are a {Role}.
Goal: {Goal}
Backstory: {Backstory}

Task Description: {task.Description}
Expected Output: {task.ExpectedOutput}

Context:
{context}

Please execute the task.
";

            var executionSettings = new OpenAIPromptExecutionSettings()
            {
                ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
            };

            var result = await scopedKernel.InvokePromptAsync(prompt, new KernelArguments(executionSettings));

            var output = result.GetValue<string>() ?? string.Empty;

            if (memoryContext != null)
            {
                memoryContext.ShortTerm.Add($"Task: {task.Description}\nResult: {output}");
                // Simple implementation: Key is task description, Content is output
                await memoryContext.LongTerm.SaveAsync(task.Description, output);
            }

            return output;
        }

        private async Task<string> BuildContextAsync(ICrewTask task, IMemoryContext? memoryContext)
        {
            if (memoryContext == null) return string.Empty;

            var shortTerm = string.Join("\n", memoryContext.ShortTerm.Get());
            var longTerm = await memoryContext.LongTerm.SearchAsync(task.Description);

            return $@"
Short Term Memory:
{shortTerm}

Long Term Memory (relevant):
{longTerm}
";
        }
    }
}
