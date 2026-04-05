using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using AgentsCrew.Core.Knowledge.ChunkingStrategies;

namespace AgentsCrew.Core.Knowledge
{
    public class XmlKnowledgeSource : BaseKnowledgeSource
    {
        private readonly string _filePath;

        public XmlKnowledgeSource(string filePath, IChunkingStrategy? chunkingStrategy = null)
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

            // Simplistic XML reader. A real scenario might traverse nodes and extract inner text.
            // Using Task.Run because XDocument.LoadAsync might not exist natively on all old profiles or we just readtext:
            var content = await File.ReadAllTextAsync(_filePath, cancellationToken);
            try
            {
                var doc = XDocument.Parse(content);
                // Return plain text of the XML values, ignoring tags
                return doc.Root?.Value ?? string.Empty;
            }
            catch
            {
                // Fallback to plain text if malformed
                return content;
            }
        }
    }
}
