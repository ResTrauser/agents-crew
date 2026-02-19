using System.Collections.Generic;
using CrewAI.DotNet.Core.Interfaces;

namespace CrewAI.DotNet.Core.Memory
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
