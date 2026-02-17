using CrewAI.DotNet.Core.Configuration;
using CrewAI.DotNet.Core.Interfaces;
using CrewAI.DotNet.Core.Models;
using CrewAI.DotNet.Core.Plugins;
using Microsoft.SemanticKernel;

namespace CrewAI.DotNet.Core.Builders
{
    public class AgentBuilder
    {
        private string _role = string.Empty;
        private string _goal = string.Empty;
        private string _backstory = string.Empty;
        private Kernel? _kernel;
        private readonly List<KernelPlugin> _tools = new();

        public AgentBuilder WithRole(string role)
        {
            _role = role;
            return this;
        }

        public AgentBuilder WithGoal(string goal)
        {
            _goal = goal;
            return this;
        }

        public AgentBuilder WithBackstory(string backstory)
        {
            _backstory = backstory;
            return this;
        }

        public AgentBuilder FromConfig(AgentConfig config)
        {
            _role = config.Role;
            _goal = config.Goal;
            _backstory = config.Backstory;
            return this;
        }

        public AgentBuilder WithKernel(Kernel kernel)
        {
            _kernel = kernel;
            return this;
        }

        public AgentBuilder AddTool(KernelPlugin tool)
        {
            _tools.Add(tool);
            return this;
        }

        public AgentBuilder WithDelegation(
            IAgentManager manager,
            IMemoryContext? memoryContext = null
        )
        {
            // We can't add plugins to _kernel directly if it's not set yet.
            // And if it is set, we modify it?
            // Agent construction logic might be cleaner if we add plugins after Kernel is created/set.
            // Let's add them to _tools list but wrapped as plugins.

            // Wait, KernelPlugin is abstract or complex. Plugins.AddFromObject creates one.
            // We need a Kernel to create a plugin? Or `KernelPluginFactory`.
            // Let's defer this. The agent needs these plugins.

            // Actually, we can just create the plugin instance and add it later.
            // But `KernelPluginFactory.CreateFromObject` creates a `KernelPlugin`.
            // It doesn't require a Kernel instance.

            var plugin = KernelPluginFactory.CreateFromObject(
                new DelegationPlugin(manager, memoryContext),
                "Delegation"
            );
            _tools.Add(plugin);
            return this;
        }

        public AgentBuilder WithAgentCreation(
            IAgentManager manager,
            Kernel baseKernel,
            IMemoryContext? memoryContext = null
        )
        {
            var plugin = KernelPluginFactory.CreateFromObject(
                new AgentCreationPlugin(manager, baseKernel, memoryContext),
                "AgentCreation"
            );
            _tools.Add(plugin);
            return this;
        }

        public IAgent Build()
        {
            var agent = new Agent(_role, _goal, _backstory, _kernel) { Tools = _tools };
            return agent;
        }
    }
}
