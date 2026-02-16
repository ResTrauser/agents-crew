using CrewAI.DotNet.Core.Interfaces;

namespace CrewAI.DotNet.Core.Models
{
    public class CrewTask : ICrewTask
    {
        public string Description { get; set; }
        public string ExpectedOutput { get; set; }
        public IAgent? AssignedAgent { get; set; }

        public CrewTask(string description, string expectedOutput, IAgent? assignedAgent = null)
        {
            Description = description;
            ExpectedOutput = expectedOutput;
            AssignedAgent = assignedAgent;
        }
    }
}
