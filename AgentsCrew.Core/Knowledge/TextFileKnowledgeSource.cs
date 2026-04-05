using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AgentsCrew.Core.Knowledge.ChunkingStrategies;

namespace AgentsCrew.Core.Knowledge
{
    public class TextFileKnowledgeSource : BaseKnowledgeSource
    {
        private readonly string _filePath;

        public TextFileKnowledgeSource(string filePath, IChunkingStrategy? chunkingStrategy = null) 
            : base(chunkingStrategy)
        {
            _filePath = filePath;
        }

        protected override async Task<string> ReadContentAsync(CancellationToken cancellationToken = default)
        {
            if (!File.Exists(_filePath))
            {
                return string.Empty;
            }

            return await File.ReadAllTextAsync(_filePath, cancellationToken);
        }
    }
}
