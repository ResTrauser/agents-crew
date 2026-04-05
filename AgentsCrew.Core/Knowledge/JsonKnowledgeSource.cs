using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AgentsCrew.Core.Knowledge.ChunkingStrategies;

namespace AgentsCrew.Core.Knowledge
{
    public class JsonKnowledgeSource : BaseKnowledgeSource
    {
        private readonly string _filePath;

        public JsonKnowledgeSource(string filePath, IChunkingStrategy? chunkingStrategy = null)
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

            // Simplistic approach: read unstructured or structured JSON and dump it to text form
            // A more advanced approach would parse specific JSON nodes.
            using FileStream stream = File.OpenRead(_filePath);
            using JsonDocument document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
            
            // Serialize back to indented format if needed, or simply stringify
            // This compresses or formats it to be processed by a chunker
            return JsonSerializer.Serialize(document.RootElement, new JsonSerializerOptions { WriteIndented = true });
        }
    }
}
