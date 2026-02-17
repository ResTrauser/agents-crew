using CrewAI.DotNet.Core.Interfaces;

namespace CrewAI.DotNet.Core.Memory
{
    public class MemoryContext(
        IShortTermMemory shortTerm,
        ILongTermMemory longTerm,
        IEntityMemory entity
    ) : IMemoryContext
    {
        public IShortTermMemory ShortTerm { get; } = shortTerm;
        public ILongTermMemory LongTerm { get; } = longTerm;
        public IEntityMemory Entity { get; } = entity;
    }
}
