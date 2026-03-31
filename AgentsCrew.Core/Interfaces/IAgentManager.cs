using System.Collections.Generic;

namespace AgentsCrew.Core.Interfaces
{
    public interface IAgentManager
    {
        IAgent? GetAgent(string role);
        void RegisterAgent(IAgent agent);
        IEnumerable<IAgent> ListAgents();
    }
}
