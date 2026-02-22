using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CrewAI.DotNet.Core.Interfaces;
using Microsoft.SemanticKernel;

namespace CrewAI.DotNet.Core.Models
{
    /// <summary>
    /// An adapter that allows using a raw Semantic Kernel instance (and optional function) as an Agent in AgentsCrew.
    /// This is useful when you have an existing SK agent or logic defined simply as a Kernel/Prompt.
    /// </summary>
    public class SemanticKernelWrapperAgent : IAgent
    {
        private readonly Kernel _kernel;
        private readonly KernelFunction? _function;

        public string Role { get; }
        public string Goal { get; }
        public string Backstory { get; }
        public int MaxIter { get; set; } = 1;
        public Action<string>? StepCallback { get; set; }
        public bool AllowDelegation { get; set; } = false;
        public bool Cache { get; set; } = false;
        public TimeSpan? MaxExecutionTime { get; set; }
        public CrewAI.DotNet.Core.Telemetry.UsageMetrics Metrics { get; } = new CrewAI.DotNet.Core.Telemetry.UsageMetrics();
        public Microsoft.Extensions.Logging.ILogger<IAgent>? Logger { get; set; }
        public IList<KernelPlugin> Tools { get; } = new List<KernelPlugin>();
        public IList<IKnowledgeSource> KnowledgeSources { get; } = new List<IKnowledgeSource>();

        /// <summary>
        /// Wraps a Kernel and uses a specific prompt template to execute tasks.
        /// </summary>
        public SemanticKernelWrapperAgent(Kernel kernel, string promptTemplate, string role = "External SK Agent", string goal = "Execute SK logic")
        {
            _kernel = kernel;
            Role = role;
            Goal = goal;
            Backstory = "Wrapped Semantic Kernel Agent";
            _function = _kernel.CreateFunctionFromPrompt(promptTemplate);
        }

        /// <summary>
        /// Wraps a Kernel and a specific existing function to execute tasks.
        /// </summary>
        public SemanticKernelWrapperAgent(Kernel kernel, KernelFunction function, string role = "External SK Agent", string goal = "Execute SK Function")
        {
            _kernel = kernel;
            _function = function;
            Role = role;
            Goal = goal;
            Backstory = "Wrapped Semantic Kernel Agent";
        }

        public async Task<string> ExecuteAsync(ICrewTask task, IMemoryContext? memoryContext = null, System.Threading.CancellationToken cancellationToken = default)
        {
            if (_function == null)
            {
                throw new InvalidOperationException("No function or prompt configured for this agent.");
            }

            // Map task description to "input" argument by default.
            var arguments = new KernelArguments
            {
                ["input"] = task.Description
            };

            // Pass context/history if the prompt supports it
            if (memoryContext != null)
            {
                 var history = string.Join("\n", memoryContext.ShortTerm.Get());
                 arguments["history"] = history;
            }

            var result = await _kernel.InvokeAsync(_function, arguments);
            var output = result.GetValue<string>() ?? string.Empty;

            task.Output = output;

            // Invoke callback if specified
            task.Callback?.Invoke(output);

            return output;
        }
    }
}
