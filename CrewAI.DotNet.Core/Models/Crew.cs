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

        // Paridad CrewAI
        public int Verbose { get; set; } = 0;
        public bool FullOutput { get; set; } = false;
        public object? ManagerLlm { get; set; } // En un futuro puede ser un objeto de configuración específico

        public Crew(IList<IAgent> agents, IList<ICrewTask> tasks, IProcess process, IMemoryContext memoryContext, IAgentManager? agentManager = null)
        {
            AgentManager = agentManager ?? new CrewAI.DotNet.Core.Process.AgentManager(agents);
            Tasks = tasks;
            Process = process;
            MemoryContext = memoryContext;
        }

        public async Task KickoffAsync()
        {
            var runner = new CrewAI.DotNet.Core.Execution.CrewRunner(this);
            await runner.StartAsync();
        }
    }
}
