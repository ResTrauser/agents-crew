using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AgentsCrew.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace AgentsCrew.Core.Process
{
    /// <summary>
    /// Executes tasks asynchronously in parallel, useful when tasks do not depend on each other's outputs.
    /// Improves performance drastically for bulk operations.
    /// </summary>
    public class AsynchronousParallelProcess : IProcess
    {
        private readonly ILogger<AsynchronousParallelProcess>? _logger;

        public AsynchronousParallelProcess(ILogger<AsynchronousParallelProcess>? logger = null)
        {
            _logger = logger;
        }

        public async Task ExecuteAsync(IList<IAgent> agents, IList<ICrewTask> tasks, IMemoryContext? memoryContext = null)
        {
            _logger?.LogInformation("Starting Parallel Execution for {TaskCount} tasks.", tasks.Count);

            var executionTasks = tasks.Select(async task =>
            {
                var agent = task.AssignedAgent ?? agents.FirstOrDefault();
                if (agent == null)
                {
                    _logger?.LogError("No agent assigned to task: {Description}", task.Description);
                    throw new InvalidOperationException($"No agent assigned to task: {task.Description}");
                }

                // Inyectar contexto si fue proveido previamente a la corrida en paralelo
                if (task.Context != null && task.Context.Any())
                {
                    var contextOutputs = task.Context
                        .Where(t => !string.IsNullOrEmpty(t.Output))
                        .Select(t => $"Output from context task '{t.Description}':\n{t.Output}");

                    if (contextOutputs.Any())
                    {
                        var appendedContext = string.Join("\n\n", contextOutputs);
                        task.Description = $"{task.Description}\n\nContexto Inicial:\n{appendedContext}";
                    }
                }

                try
                {
                    var output = await agent.ExecuteAsync(task, memoryContext);
                    task.OnTaskCompleted?.Invoke(task);
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Failed to execute parallel task: {Description}", task.Description);
                    // Depending on policy, we might want to fail the whole batch or let others finish
                    throw; 
                }
            });

            await Task.WhenAll(executionTasks);
            
            _logger?.LogInformation("Parallel Execution Completed.");
        }
    }
}
