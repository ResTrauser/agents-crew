using System.Collections.Generic;

namespace CrewAI.DotNet.Core.Interfaces
{
    public interface IShortTermMemory
    {
        void Add(string content);
        IEnumerable<string> Get();
        void Clear();
    }
}
