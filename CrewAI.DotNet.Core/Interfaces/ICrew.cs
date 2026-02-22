using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrewAI.DotNet.Core.Interfaces
{
    public interface ICrew
    {
        IList<IAgent> Agents { get; }
        IList<ICrewTask> Tasks { get; }
        IProcess Process { get; }
        IMemoryContext MemoryContext { get; }
        IAgentManager AgentManager { get; }

        // Paridad CrewAI
        int Verbose { get; set; }
        bool FullOutput { get; set; }
        object? ManagerLlm { get; set; } // En un futuro puede ser un objeto de configuración específico

        Task KickoffAsync();
    }
}
