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
        public int MaxIter { get; set; }
        public Action<string>? StepCallback { get; set; }
        public bool AllowDelegation { get; set; }
        public bool Cache { get; set; }
        public IList<KernelPlugin> Tools { get; set; } = new List<KernelPlugin>();
        public IList<IKnowledgeSource> KnowledgeSources { get; set; } = new List<IKnowledgeSource>();

        public Kernel? Kernel { get; set; }

        public Agent(string role, string goal, string backstory, Kernel? kernel = null, int maxIter = 25, Action<string>? stepCallback = null, bool allowDelegation = true, bool cache = true)
        {
            Role = role;
            Goal = goal;
            Backstory = backstory;
            Kernel = kernel;
            MaxIter = maxIter;
            StepCallback = stepCallback;
            AllowDelegation = allowDelegation;
            Cache = cache;
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

            if (memoryContext != null && !_knowledgeIngested)
            {
                await IngestKnowledgeAsync(memoryContext);
                _knowledgeIngested = true;
            }

            var context = await BuildContextAsync(task, memoryContext);

            var prompt = $@"
You are a {Role}.
Goal: {Goal}
Backstory: {Backstory}

Task Description: {task.Description}
Expected Output: {task.ExpectedOutput}
{GetStructuredOutputInstruction(task)}

Context:
{context}

Please execute the task.
";

            // Use generic PromptExecutionSettings if possible, or explicit OpenAIPromptExecutionSettings
            // Note: For custom services (like Mocks), auto-invocation might depend on the service handling it
            // or the use of FunctionChoiceBehavior in newer SK versions.
            // Falling back to manual loop if auto-invoke is not supported by the service is complex.
            // Assuming OpenAIPromptExecutionSettings works with the underlying mechanism (filters).

            var executionSettings = new OpenAIPromptExecutionSettings()
            {
                ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
            };

            var result = await scopedKernel.InvokePromptAsync(prompt, new KernelArguments(executionSettings));

            var output = result.GetValue<string>() ?? string.Empty;

            // Set task output
            task.Output = output;

            // Save to output file if specified
            if (!string.IsNullOrEmpty(task.OutputFile))
            {
                try
                {
                    System.IO.File.WriteAllText(task.OutputFile, output);
                }
                catch (Exception ex)
                {
                    // Log or ignore? For now ignore but maybe Console.WriteLine
                    Console.WriteLine($"Failed to write output file: {ex.Message}");
                }
            }

            // Invoke callback if specified
            task.Callback?.Invoke(output);

            if (memoryContext != null)
            {
                memoryContext.ShortTerm.Add($"Task: {task.Description}\nResult: {output}");
                // Simple implementation: Key is task description, Content is output
                await memoryContext.LongTerm.SaveAsync(task.Description, output);
            }

            return output;
        }

        private bool _knowledgeIngested = false;

        private async Task IngestKnowledgeAsync(IMemoryContext memoryContext)
        {
            foreach (var source in KnowledgeSources)
            {
                var chunks = await source.GetContentChunksAsync();
                foreach (var chunk in chunks)
                {
                    var key = Guid.NewGuid().ToString();
                    await memoryContext.LongTerm.SaveAsync(key, chunk);
                }
            }
        }

        private string GetStructuredOutputInstruction(ICrewTask task)
        {
            if (task.OutputType == null) return string.Empty;

            // Generate a JSON schema or simple description of the type
            // For MVP, we'll just ask for JSON matching the properties.
            var properties = task.OutputType.GetProperties().Select(p => $"{p.Name} ({p.PropertyType.Name})");
            return $@"
IMPORTANT: You MUST return the result as a valid JSON object matching this schema:
{{
  {string.Join(",\n  ", properties)}
}}
Do not include any markdown formatting (like ```json). Just the raw JSON string.
";
        }

        private async Task<string> BuildContextAsync(ICrewTask task, IMemoryContext? memoryContext)
        {
            var contextBuilder = new System.Text.StringBuilder();

            // Include Task Context
            if (task.Context != null && task.Context.Count > 0)
            {
                contextBuilder.AppendLine("Previous Tasks Context:");
                foreach (var ctxTask in task.Context)
                {
                    if (!string.IsNullOrEmpty(ctxTask.Output))
                    {
                        contextBuilder.AppendLine($"Task: {ctxTask.Description}");
                        contextBuilder.AppendLine($"Output: {ctxTask.Output}");
                        contextBuilder.AppendLine("---");
                    }
                }
            }

            if (memoryContext != null)
            {
                var shortTerm = string.Join("\n", memoryContext.ShortTerm.Get());
                var longTerm = await memoryContext.LongTerm.SearchAsync(task.Description);

                contextBuilder.AppendLine("Short Term Memory:");
                contextBuilder.AppendLine(shortTerm);
                contextBuilder.AppendLine("Long Term Memory (relevant):");
                contextBuilder.AppendLine(longTerm);
            }

            return contextBuilder.ToString();
        }
    }
}
