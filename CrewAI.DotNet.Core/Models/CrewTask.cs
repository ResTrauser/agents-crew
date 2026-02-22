using CrewAI.DotNet.Core.Interfaces;

namespace CrewAI.DotNet.Core.Models
{
    public class CrewTask : ICrewTask
    {
        public string Description { get; set; }
        public string ExpectedOutput { get; set; }
        public IAgent? AssignedAgent { get; set; }
        public System.Type? OutputType { get; set; }

        // Paridad CrewAI
        public System.Collections.Generic.IList<ICrewTask> Context { get; set; } = new System.Collections.Generic.List<ICrewTask>();
        public string? OutputFile { get; set; }
        public bool AsyncExecution { get; set; }
        public bool HumanInput { get; set; }
        public System.Action<ICrewTask>? OnTaskCompleted { get; set; }

        public CrewTask(string description, string expectedOutput, IAgent? assignedAgent = null, System.Type? outputType = null)
        {
            Description = description;
            ExpectedOutput = expectedOutput;
            AssignedAgent = assignedAgent;
            OutputType = outputType;
        }
    }
}
