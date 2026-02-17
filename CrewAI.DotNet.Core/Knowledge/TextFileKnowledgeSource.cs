using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CrewAI.DotNet.Core.Interfaces;

namespace CrewAI.DotNet.Core.Knowledge
{
    public class TextFileKnowledgeSource : IKnowledgeSource
    {
        private readonly string _filePath;

        public TextFileKnowledgeSource(string filePath)
        {
            _filePath = filePath;
        }

        public async Task<IEnumerable<string>> GetContentChunksAsync()
        {
            if (!File.Exists(_filePath))
            {
                return new List<string>();
            }

            var content = await File.ReadAllTextAsync(_filePath);

            // Simple chunking strategy: Split by paragraphs for now.
            // In a real system, we'd use a more sophisticated chunker (token-based).
            var chunks = content.Split(new[] { "\n\n" }, System.StringSplitOptions.RemoveEmptyEntries);

            return chunks;
        }
    }
}
