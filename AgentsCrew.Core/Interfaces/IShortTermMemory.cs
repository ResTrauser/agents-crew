using System.Collections.Generic;

namespace AgentsCrew.Core.Interfaces
{
    public interface IShortTermMemory
    {
        void Add(string content);
        IEnumerable<string> Get();
        void Clear();
    }
}
