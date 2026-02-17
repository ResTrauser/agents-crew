using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Memory;

namespace CrewAI.DotNet.Core.Memory
{
#pragma warning disable SKEXP0001
    public class VolatileSemanticMemory : ISemanticTextMemory
    {
        private readonly Dictionary<string, List<(string Text, string Id)>> _storage = new();

        public Task<string> SaveInformationAsync(string collection, string text, string id, string? description = null, string? additionalMetadata = null, Kernel? kernel = null, CancellationToken cancellationToken = default)
        {
            if (!_storage.ContainsKey(collection))
            {
                _storage[collection] = new List<(string, string)>();
            }
            _storage[collection].Add((text, id));
            return Task.FromResult(id);
        }

        public Task<string> SaveReferenceAsync(string collection, string text, string externalId, string externalSourceName, string? description = null, string? additionalMetadata = null, Kernel? kernel = null, CancellationToken cancellationToken = default)
        {
             return SaveInformationAsync(collection, text, externalId, description, additionalMetadata, kernel, cancellationToken);
        }

        public Task<MemoryQueryResult?> GetAsync(string collection, string key, bool withEmbedding = false, Kernel? kernel = null, CancellationToken cancellationToken = default)
        {
             if (_storage.TryGetValue(collection, out var items))
            {
                var item = items.FirstOrDefault(i => i.Id == key);
                if (item != default)
                {
                    return Task.FromResult<MemoryQueryResult?>(new MemoryQueryResult(new MemoryRecordMetadata(true, item.Id, item.Text, string.Empty, string.Empty, string.Empty), 1.0, null));
                }
            }
            return Task.FromResult<MemoryQueryResult?>(null);
        }

        public Task RemoveAsync(string collection, string key, Kernel? kernel = null, CancellationToken cancellationToken = default)
        {
             if (_storage.TryGetValue(collection, out var items))
            {
                var item = items.FirstOrDefault(i => i.Id == key);
                if (item != default)
                {
                    items.Remove(item);
                }
            }
            return Task.CompletedTask;
        }

        public async IAsyncEnumerable<MemoryQueryResult> SearchAsync(string collection, string query, int limit = 1, double minRelevanceScore = 0.7, bool withEmbeddings = false, Kernel? kernel = null, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
             if (_storage.TryGetValue(collection, out var items))
            {
                var results = items
                    // Simple substring search for MVP
                    .Where(i => i.Text.Contains(query, System.StringComparison.OrdinalIgnoreCase))
                    .Take(limit)
                    .Select(i => new MemoryQueryResult(new MemoryRecordMetadata(true, i.Id, i.Text, string.Empty, string.Empty, string.Empty), 1.0, null));

                foreach (var result in results)
                {
                    yield return result;
                }
            }
            await Task.CompletedTask;
        }

        public Task<IList<string>> GetCollectionsAsync(Kernel? kernel = null, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IList<string>>((IList<string>)_storage.Keys.ToList());
        }
    }
#pragma warning restore SKEXP0001
}
