using AgentsCrew.Core.Interfaces;

namespace AgentsCrew.Core.Memory
{
    public class MemoryContext : IMemoryContext
    {
        public IShortTermMemory ShortTerm { get; }
        public ILongTermMemory LongTerm { get; }
        public IEntityMemory Entity { get; }

        public MemoryContext(IShortTermMemory shortTerm, ILongTermMemory longTerm, IEntityMemory entity)
        {
            ShortTerm = shortTerm;
            LongTerm = longTerm;
            Entity = entity;
        }
    }
}
