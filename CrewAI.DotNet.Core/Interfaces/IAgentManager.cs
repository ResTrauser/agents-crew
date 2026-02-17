using System.Collections.Generic;

namespace CrewAI.DotNet.Core.Interfaces
{
    public interface IAgentManager
    {
        IAgent? GetAgent(string role);
        void RegisterAgent(IAgent agent);
        IEnumerable<IAgent> ListAgents();
    }
}
