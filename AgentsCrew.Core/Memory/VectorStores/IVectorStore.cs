using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AgentsCrew.Core.Memory.VectorStores
{
    public interface IVectorStore
    {
        Task<string> UpsertAsync(string collection, IVectorRecord record, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<IVectorRecord>> SearchAsync(string collection, ReadOnlyMemory<float> embedding, int topK = 4, CancellationToken cancellationToken = default);
        Task DeleteAsync(string collection, string id, CancellationToken cancellationToken = default);
        Task<bool> CollectionExistsAsync(string collection, CancellationToken cancellationToken = default);
        Task CreateCollectionAsync(string collection, int dimensions, CancellationToken cancellationToken = default);
    }
}
