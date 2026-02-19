using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrewAI.DotNet.Core.Interfaces;

namespace CrewAI.DotNet.Core.Process
{
    public class SequentialProcess : IProcess
    {
        public async Task ExecuteAsync(IList<IAgent> agents, IList<ICrewTask> tasks, IMemoryContext? memoryContext = null)
        {
            foreach (var task in tasks)
            {
                var agent = task.AssignedAgent ?? agents.FirstOrDefault();
                if (agent == null)
                {
                    throw new InvalidOperationException($"No agent assigned to task: {task.Description}");
                }

                // Execute task
                await agent.ExecuteAsync(task, memoryContext);
            }
        }
    }
}
