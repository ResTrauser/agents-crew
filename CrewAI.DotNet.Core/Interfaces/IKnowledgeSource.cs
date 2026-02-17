namespace CrewAI.DotNet.Core.Interfaces
{
    public interface IKnowledgeSource
    {
        Task<IEnumerable<string>> GetContentChunksAsync();
    }
}
