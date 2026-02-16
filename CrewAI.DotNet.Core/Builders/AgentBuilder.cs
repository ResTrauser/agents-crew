using System.Collections.Generic;
using CrewAI.DotNet.Core.Interfaces;
using CrewAI.DotNet.Core.Models;
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

        public IAgent Build()
        {
            var agent = new Agent(_role, _goal, _backstory, _kernel)
            {
                Tools = _tools
            };
            return agent;
        }
    }
}
