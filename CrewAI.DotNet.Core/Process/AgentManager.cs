using System.Collections.Concurrent;
using CrewAI.DotNet.Core.Interfaces;

namespace CrewAI.DotNet.Core.Process
{
    public class AgentManager : IAgentManager
    {
        private readonly ConcurrentDictionary<string, IAgent> _agents = new();

        public AgentManager(IEnumerable<IAgent>? initialAgents = null)
        {
            if (initialAgents != null)
            {
                foreach (var agent in initialAgents)
                {
                    _agents.TryAdd(agent.Role, agent);
                }
            }
        }

        public IAgent? GetAgent(string role)
        {
            _agents.TryGetValue(role, out var agent);
            return agent;
        }

        public void RegisterAgent(IAgent agent)
        {
            _agents.TryAdd(agent.Role, agent);
        }

        public IEnumerable<IAgent> ListAgents()
        {
            return _agents.Values;
        }
    }
}
