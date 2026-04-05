using System.Collections.Generic;
using AgentsCrew.Core.Interfaces;

namespace AgentsCrew.Core.Memory
{
    public class ShortTermMemory : IShortTermMemory
    {
        private readonly List<string> _memory = new();

        public void Add(string content)
        {
            _memory.Add(content);
        }

        public IEnumerable<string> Get()
        {
            return _memory;
        }

        public void Clear()
        {
            _memory.Clear();
        }
    }
}
