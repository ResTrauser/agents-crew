using System.Collections.Generic;

namespace CrewAI.DotNet.Core.Interfaces
{
    public interface ICrewTask
    {
        string Description { get; set; }
        string ExpectedOutput { get; }
        IAgent? AssignedAgent { get; set; }
        System.Type? OutputType { get; set; }

        // Paridad CrewAI
        System.Collections.Generic.IList<ICrewTask> Context { get; set; }
        string? OutputFile { get; set; }
        bool AsyncExecution { get; set; }
        bool HumanInput { get; set; }
        System.Action<ICrewTask>? OnTaskCompleted { get; set; }
        IList<ICrewTask>? Context { get; }
        string? OutputFile { get; }
        System.Action<string>? Callback { get; }
        bool AsyncExecution { get; }
        string? Output { get; set; }
    }
}
