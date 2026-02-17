using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrewAI.DotNet.Core.Interfaces;
using CrewAI.DotNet.Core.Models;
using Microsoft.SemanticKernel;

namespace CrewAI.DotNet.Core.Process
{
    public class HierarchicalProcess : IProcess
    {
        private readonly IAgent _managerAgent;

        public HierarchicalProcess(IAgent managerAgent)
        {
            _managerAgent = managerAgent;
        }

        public async Task ExecuteAsync(IList<IAgent> agents, IList<ICrewTask> tasks, IMemoryContext? memoryContext = null)
        {
            foreach (var task in tasks)
            {
                // In a hierarchical process, unassigned tasks default to the manager
                // who is expected to delegate them using the DelegationPlugin.
                var executingAgent = task.AssignedAgent ?? _managerAgent;

                // Ensure the executing agent (Manager) has access to delegate to the crew agents
                // This requires that the Manager was configured with the AgentManager containing these agents.
                // Assuming the Crew setup handled this or the user configured the Manager correctly.

                await executingAgent.ExecuteAsync(task, memoryContext);
            }
        }
    }
}
