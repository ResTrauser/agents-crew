using System;
using System.Collections.Generic;
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
        private IList<ICrewTask>? _context;
        private string? _outputFile;
        private Action<string>? _callback;
        private bool _asyncExecution = false;

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

        public CrewTaskBuilder WithContext(IList<ICrewTask> context)
        {
            _context = context;
            return this;
        }

        public CrewTaskBuilder WithOutputFile(string outputFile)
        {
            _outputFile = outputFile;
            return this;
        }

        public CrewTaskBuilder WithCallback(Action<string> callback)
        {
            _callback = callback;
            return this;
        }

        public CrewTaskBuilder WithAsyncExecution(bool asyncExecution)
        {
            _asyncExecution = asyncExecution;
            return this;
        }

        public ICrewTask Build()
        {
            return new CrewTask(_description, _expectedOutput, _assignedAgent, null, _context, _outputFile, _callback, _asyncExecution);
        }
    }
}
