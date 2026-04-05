using AgentsCrew.Core.Interfaces;

namespace AgentsCrew.Core.Memory
{
    public class MemoryContext : IMemoryContext
    {
        public IShortTermMemory ShortTerm { get; }
        public ILongTermMemory LongTerm { get; }
        public IEntityMemory Entity { get; }
        public ISessionMemory Session { get; }

        public MemoryContext(IShortTermMemory shortTerm, ILongTermMemory longTerm, IEntityMemory entity, ISessionMemory session)
        {
            ShortTerm = shortTerm;
            LongTerm = longTerm;
            Entity = entity;
            Session = session;
        }
    }
}
