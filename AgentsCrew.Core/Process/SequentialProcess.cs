using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AgentsCrew.Core.Interfaces;

namespace AgentsCrew.Core.Process
{
    public class SequentialProcess : IProcess
    {
        public async Task ExecuteAsync(IList<IAgent> agents, IList<ICrewTask> tasks, IMemoryContext? memoryContext = null)
        {
            var taskResults = new Dictionary<ICrewTask, string>();

            foreach (var task in tasks)
            {
                var agent = task.AssignedAgent ?? agents.FirstOrDefault();
                if (agent == null)
                {
                    throw new InvalidOperationException($"No agent assigned to task: {task.Description}");
                }

                // Inyectar contexto de tareas previas si la tarea actual lo define
                if (task.Context != null && task.Context.Any())
                {
                    var contextOutputs = task.Context
                        .Where(t => taskResults.ContainsKey(t))
                        .Select(t => $"Output from previous task '{t.Description}':\n{taskResults[t]}");

                    if (contextOutputs.Any())
                    {
                        var appendedContext = string.Join("\n\n", contextOutputs);
                        // Dependiendo de si la descripción se puede alterar o si hay un mejor lugar para el RuntimeContext.
                        // Modificamos la descripción on-the-fly para que Semantic Kernel lo reciba.
                        task.Description = $"{task.Description}\n\nContexto Inicial:\n{appendedContext}";
                    }
                }

                // Execute task
                var output = await agent.ExecuteAsync(task, memoryContext);
                
                // Guardar resultado y disparar evento de completitud local
                taskResults[task] = output;
                task.OnTaskCompleted?.Invoke(task);
            }
        }
    }
}
