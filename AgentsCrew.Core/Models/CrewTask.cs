using AgentsCrew.Core.Interfaces;

namespace AgentsCrew.Core.Models
{
    public class CrewTask : ICrewTask
    {
        public string Description { get; set; }
        public string ExpectedOutput { get; set; }
        public IAgent? AssignedAgent { get; set; }
        public System.Type? OutputType { get; set; }

        public CrewTask(string description, string expectedOutput, IAgent? assignedAgent = null, System.Type? outputType = null)
        {
            Description = description;
            ExpectedOutput = expectedOutput;
            AssignedAgent = assignedAgent;
            OutputType = outputType;
        }
    }
}
