namespace CrewAI.DotNet.Core.Configuration
{
    public class TaskConfig
    {
        public string Description { get; set; } = string.Empty;
        public string ExpectedOutput { get; set; } = string.Empty;
        public string AssignedAgent { get; set; } = string.Empty; // Reference by Role or Name
    }
}
