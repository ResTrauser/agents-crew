using System.Collections.Generic;
using AgentsCrew.Core.Interfaces;
using AgentsCrew.Core.Models;
using AgentsCrew.Core.Process;
using AgentsCrew.Core.Memory;
using Microsoft.SemanticKernel.Memory;

namespace AgentsCrew.Core.Builders
{
    public class CrewBuilder
    {
        private readonly List<IAgent> _agents = new();
        private readonly List<ICrewTask> _tasks = new();
        private IProcess? _process;
        private IMemoryContext? _memoryContext;

        public CrewBuilder AddAgent(IAgent agent)
        {
            _agents.Add(agent);
            return this;
        }

        public CrewBuilder AddTask(ICrewTask task)
        {
            _tasks.Add(task);
            return this;
        }

        public CrewBuilder WithProcess(IProcess process)
        {
            _process = process;
            return this;
        }

        public CrewBuilder WithMemory(IMemoryContext memoryContext)
        {
            _memoryContext = memoryContext;
            return this;
        }

        public ICrew Build()
        {
            var process = _process ?? new SequentialProcess();

            IMemoryContext memory;
            if (_memoryContext != null)
            {
                memory = _memoryContext;
            }
            else
            {
#pragma warning disable SKEXP0001
                var semanticMemory = new VolatileSemanticMemory(); // Simple in-memory mock

                memory = new MemoryContext(
                    new ShortTermMemory(),
                    new LongTermMemory(semanticMemory),
                    new EntityMemory()
                );
#pragma warning restore SKEXP0001
            }

            return new Crew(_agents, _tasks, process, memory);
        }
    }
}
