namespace AgentsCrew.Core.Interfaces
{
    public interface IMemoryContext
    {
        IShortTermMemory ShortTerm { get; }
        ILongTermMemory LongTerm { get; }
        IEntityMemory Entity { get; }
    }
}
