using CrewAI.DotNet.Core.Interfaces;

namespace CrewAI.DotNet.Core.Models
{
    public class Crew(
        IList<IAgent> agents,
        IList<ICrewTask> tasks,
        IProcess process,
        IMemoryContext memoryContext,
        IAgentManager? agentManager = null
    ) : ICrew
    {
        public IList<IAgent> Agents => AgentManager.ListAgents().ToList();
        public IList<ICrewTask> Tasks { get; set; } = tasks;
        public IProcess Process { get; set; } = process;
        public IMemoryContext MemoryContext { get; set; } = memoryContext;
        public IAgentManager AgentManager { get; } =
            agentManager ?? new CrewAI.DotNet.Core.Process.AgentManager(agents);

        public async Task KickoffAsync()
        {
            await Process.ExecuteAsync(Agents, Tasks, MemoryContext);
        }
    }
}
