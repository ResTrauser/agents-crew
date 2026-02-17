using CrewAI.DotNet.Core.Interfaces;

namespace CrewAI.DotNet.Core.Models
{
    public class CrewTask(
        string description,
        string expectedOutput,
        IAgent? assignedAgent = null,
        Type? outputType = null
    ) : ICrewTask
    {
        public string Description { get; set; } = description;
        public string ExpectedOutput { get; set; } = expectedOutput;
        public IAgent? AssignedAgent { get; set; } = assignedAgent;
        public Type? OutputType { get; set; } = outputType;
    }
}
