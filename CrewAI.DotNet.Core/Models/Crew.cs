using System.Collections.Generic;
using System.Threading.Tasks;
using CrewAI.DotNet.Core.Interfaces;

namespace CrewAI.DotNet.Core.Models
{
    public class Crew : ICrew
    {
        public IList<IAgent> Agents => AgentManager.ListAgents().ToList();
        public IList<ICrewTask> Tasks { get; set; }
        public IProcess Process { get; set; }
        public IMemoryContext MemoryContext { get; set; }
        public IAgentManager AgentManager { get; }

        public Crew(IList<IAgent> agents, IList<ICrewTask> tasks, IProcess process, IMemoryContext memoryContext, IAgentManager? agentManager = null)
        {
            AgentManager = agentManager ?? new CrewAI.DotNet.Core.Process.AgentManager(agents);
            Tasks = tasks;
            Process = process;
            MemoryContext = memoryContext;
        }

        public async Task KickoffAsync()
        {
            await Process.ExecuteAsync(Agents, Tasks, MemoryContext);
        }
    }
}
