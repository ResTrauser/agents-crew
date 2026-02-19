using AgentsCrew.Core.Interfaces;
using AgentsCrew.Core.Models;
using AgentsCrew.Core.Configuration;

namespace AgentsCrew.Core.Builders
{
    public class CrewTaskBuilder
    {
        private string _description = string.Empty;
        private string _expectedOutput = string.Empty;
        private IAgent? _assignedAgent;

        public CrewTaskBuilder WithDescription(string description)
        {
            _description = description;
            return this;
        }

        public CrewTaskBuilder WithExpectedOutput(string expectedOutput)
        {
            _expectedOutput = expectedOutput;
            return this;
        }

        public CrewTaskBuilder FromConfig(TaskConfig config)
        {
            _description = config.Description;
            _expectedOutput = config.ExpectedOutput;
            return this;
        }

        public CrewTaskBuilder AssignTo(IAgent agent)
        {
            _assignedAgent = agent;
            return this;
        }

        public ICrewTask Build()
        {
            return new CrewTask(_description, _expectedOutput, _assignedAgent);
        }
    }
}
