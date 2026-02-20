using System.Collections.Generic;

namespace CrewAI.DotNet.Core.Interfaces
{
    public interface ICrewTask
    {
        string Description { get; }
        string ExpectedOutput { get; }
        IAgent? AssignedAgent { get; set; }
        System.Type? OutputType { get; set; }
        IList<ICrewTask>? Context { get; }
        string? OutputFile { get; }
        System.Action<string>? Callback { get; }
        bool AsyncExecution { get; }
        string? Output { get; set; }
    }
}
