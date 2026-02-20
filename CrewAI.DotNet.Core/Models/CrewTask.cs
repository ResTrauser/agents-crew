using System;
using System.Collections.Generic;
using CrewAI.DotNet.Core.Interfaces;

namespace CrewAI.DotNet.Core.Models
{
    public class CrewTask : ICrewTask
    {
        public string Description { get; set; }
        public string ExpectedOutput { get; set; }
        public IAgent? AssignedAgent { get; set; }
        public System.Type? OutputType { get; set; }
        public IList<ICrewTask>? Context { get; set; }
        public string? OutputFile { get; set; }
        public Action<string>? Callback { get; set; }
        public bool AsyncExecution { get; set; }
        public string? Output { get; set; }

        public CrewTask(string description, string expectedOutput, IAgent? assignedAgent = null, System.Type? outputType = null, IList<ICrewTask>? context = null, string? outputFile = null, Action<string>? callback = null, bool asyncExecution = false)
        {
            Description = description;
            ExpectedOutput = expectedOutput;
            AssignedAgent = assignedAgent;
            OutputType = outputType;
            Context = context;
            OutputFile = outputFile;
            Callback = callback;
            AsyncExecution = asyncExecution;
        }
    }
}
