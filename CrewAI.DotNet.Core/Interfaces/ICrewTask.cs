namespace CrewAI.DotNet.Core.Interfaces
{
    public interface ICrewTask
    {
        string Description { get; }
        string ExpectedOutput { get; }
        IAgent? AssignedAgent { get; set; }
    }
}
