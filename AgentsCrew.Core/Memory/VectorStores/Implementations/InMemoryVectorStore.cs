using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AgentsCrew.Core.Memory.VectorStores.Implementations
{
    public class InMemoryVectorStore : IVectorStore
    {
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, IVectorRecord>> _collections = new();

        public Task<string> UpsertAsync(string collection, IVectorRecord record, CancellationToken cancellationToken = default)
        {
            if (!_collections.ContainsKey(collection))
            {
                _collections[collection] = new ConcurrentDictionary<string, IVectorRecord>();
            }

            _collections[collection][record.Id] = record;
            return Task.FromResult(record.Id);
        }

        public Task<IReadOnlyList<IVectorRecord>> SearchAsync(string collection, ReadOnlyMemory<float> embedding, int topK = 4, CancellationToken cancellationToken = default)
        {
            if (!_collections.ContainsKey(collection))
            {
                return Task.FromResult<IReadOnlyList<IVectorRecord>>(Array.Empty<IVectorRecord>());
            }

            var results = _collections[collection].Values
                .Select(record => new { Record = record, Score = CosineSimilarity(embedding, record.Embedding) })
                .OrderByDescending(x => x.Score)
                .Take(topK)
                .Select(x => x.Record)
                .ToList();

            return Task.FromResult<IReadOnlyList<IVectorRecord>>(results);
        }

        public Task DeleteAsync(string collection, string id, CancellationToken cancellationToken = default)
        {
            if (_collections.ContainsKey(collection))
            {
                _collections[collection].TryRemove(id, out _);
            }
            return Task.CompletedTask;
        }

        public Task<bool> CollectionExistsAsync(string collection, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_collections.ContainsKey(collection));
        }

        public Task CreateCollectionAsync(string collection, int dimensions, CancellationToken cancellationToken = default)
        {
            _collections[collection] = new ConcurrentDictionary<string, IVectorRecord>();
            return Task.CompletedTask;
        }

        private static float CosineSimilarity(ReadOnlyMemory<float> vector1, ReadOnlyMemory<float> vector2)
        {
            var span1 = vector1.Span;
            var span2 = vector2.Span;

            if (span1.Length != span2.Length)
            {
                throw new ArgumentException("Vectors must have the same length");
            }

            float dotProduct = 0;
            float magnitude1 = 0;
            float magnitude2 = 0;

            for (int i = 0; i < span1.Length; i++)
            {
                dotProduct += span1[i] * span2[i];
                magnitude1 += span1[i] * span1[i];
                magnitude2 += span2[i] * span2[i];
            }

            if (magnitude1 == 0 || magnitude2 == 0)
            {
                return 0;
            }

            return dotProduct / (MathF.Sqrt(magnitude1) * MathF.Sqrt(magnitude2));
        }
    }
}
