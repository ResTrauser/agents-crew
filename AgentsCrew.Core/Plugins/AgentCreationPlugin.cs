using System.ComponentModel;
using AgentsCrew.Core.Builders;
using AgentsCrew.Core.Interfaces;
using AgentsCrew.Core.Models;
using Microsoft.SemanticKernel;

namespace AgentsCrew.Core.Plugins
{
    public class AgentCreationPlugin
    {
        private readonly IAgentManager _agentManager;
        private readonly Kernel _baseKernel;
        private readonly IMemoryContext? _memoryContext;

        public AgentCreationPlugin(IAgentManager agentManager, Kernel baseKernel, IMemoryContext? memoryContext = null)
        {
            _agentManager = agentManager;
            _baseKernel = baseKernel;
            _memoryContext = memoryContext;
        }

        [KernelFunction]
        [Description("Creates a new agent with a specific role, goal, and backstory.")]
        public string CreateAgent(
            [Description("The role of the new agent.")] string role,
            [Description("The goal of the new agent.")] string goal,
            [Description("The backstory of the new agent.")] string backstory)
        {
            // Create a new kernel for the agent, potentially inheriting services from the base kernel
            // We clone the base kernel to share services like AI connectors
            var newAgentKernel = _baseKernel.Clone();

            // Add delegation capability to the new agent
            // This enables recursive delegation
            var delegationPlugin = new DelegationPlugin(_agentManager, _memoryContext);
            newAgentKernel.Plugins.AddFromObject(delegationPlugin, "Delegation");

            // Add creation capability to the new agent (recursion)
            // Note: passing 'this' might be tricky due to state, so we create a new plugin instance or pass dependencies
            // Ideally we'd use DI, but here we manually wire up.
            // Be careful about infinite recursion if an agent creates an agent that creates an agent...
            var creationPlugin = new AgentCreationPlugin(_agentManager, _baseKernel, _memoryContext);
            newAgentKernel.Plugins.AddFromObject(creationPlugin, "AgentCreation");

            var agent = new AgentBuilder()
                .WithRole(role)
                .WithGoal(goal)
                .WithBackstory(backstory)
                .WithKernel(newAgentKernel)
                .Build();

            _agentManager.RegisterAgent(agent);

            return $"Agent '{role}' created successfully.";
        }
    }
}
