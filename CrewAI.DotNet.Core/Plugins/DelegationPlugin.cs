using System.ComponentModel;
using System.Threading.Tasks;
using CrewAI.DotNet.Core.Interfaces;
using CrewAI.DotNet.Core.Models;
using Microsoft.SemanticKernel;

namespace CrewAI.DotNet.Core.Plugins
{
    public class DelegationPlugin
    {
        private readonly IAgentManager _agentManager;
        private readonly IMemoryContext? _memoryContext;

        public DelegationPlugin(IAgentManager agentManager, IMemoryContext? memoryContext = null)
        {
            _agentManager = agentManager;
            _memoryContext = memoryContext;
        }

        [KernelFunction]
        [Description("Delegates a task to another agent. Use this when you need assistance from a specific agent.")]
        public async Task<string> DelegateTask(
            [Description("The role or name of the agent to delegate to.")] string agentRole,
            [Description("The description of the task to be performed.")] string taskDescription)
        {
            var agent = _agentManager.GetAgent(agentRole);
            if (agent == null)
            {
                return $"Error: Agent with role '{agentRole}' not found. Available agents: {string.Join(", ", _agentManager.ListAgents())}";
            }

            var task = new CrewTask(taskDescription, "Result of delegated task", agent);

            // Execute the task
            // Note: We might want to pass the current memory context
            var result = await agent.ExecuteAsync(task, _memoryContext);

            return $"Task delegated to {agentRole}. Result: {result}";
        }

        [KernelFunction]
        [Description("Lists available agents.")]
        public string ListAgents()
        {
            var agents = _agentManager.ListAgents();
            return string.Join(", ", System.Linq.Enumerable.Select(agents, a => a.Role));
        }
    }
}
