using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AgentsCrew.Core.Embeddings.Interfaces
{
    public interface IEmbeddingGenerator
    {
        Task<ReadOnlyMemory<float>> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<ReadOnlyMemory<float>>> GenerateEmbeddingsAsync(IEnumerable<string> texts, CancellationToken cancellationToken = default);
        int Dimensions { get; }
    }
}
