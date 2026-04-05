using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AgentsCrew.Core.Knowledge.ChunkingStrategies;

namespace AgentsCrew.Core.Knowledge
{
    /// <summary>
    /// Abstract base class for knowledge sources that standardizes the chunking mechanism.
    /// </summary>
    public abstract class BaseKnowledgeSource : IKnowledgeSource
    {
        protected readonly IChunkingStrategy ChunkingStrategy;

        protected BaseKnowledgeSource(IChunkingStrategy? chunkingStrategy = null)
        {
            // Default to FixedSizeChunker if none is provided
            ChunkingStrategy = chunkingStrategy ?? new FixedSizeChunker(1000, 100);
        }

        /// <summary>
        /// Reads the full raw content from the underlying source.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The raw text content.</returns>
        protected abstract Task<string> ReadContentAsync(CancellationToken cancellationToken = default);

        public async Task<IEnumerable<string>> GetContentChunksAsync(CancellationToken cancellationToken = default)
        {
            var rawContent = await ReadContentAsync(cancellationToken);

            if (string.IsNullOrWhiteSpace(rawContent))
            {
                return Array.Empty<string>();
            }

            return ChunkingStrategy.Chunk(rawContent);
        }
    }
}
