using CrewAI.DotNet.Core.Interfaces;

namespace CrewAI.DotNet.Core.Knowledge
{
    public class TextFileKnowledgeSource(string filePath) : IKnowledgeSource
    {
        private readonly string _filePath = filePath;

        public async Task<IEnumerable<string>> GetContentChunksAsync()
        {
            if (!File.Exists(_filePath))
            {
                return new List<string>();
            }

            var content = await File.ReadAllTextAsync(_filePath);

            // Simple chunking strategy: Split by paragraphs for now.
            // In a real system, we'd use a more sophisticated chunker (token-based).
            var chunks = content.Split(["\n\n"], StringSplitOptions.RemoveEmptyEntries);

            return chunks;
        }
    }
}
