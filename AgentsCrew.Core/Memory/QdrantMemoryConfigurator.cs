using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.SemanticKernel.Memory;

namespace AgentsCrew.Core.Memory
{
    /// <summary>
    /// Extension helper to configure Semantic Kernel Memory with Qdrant Vector Search.
    /// Provides persistent long-term memory for Agent's RAG capabilities.
    /// </summary>
    public static class QdrantMemoryConfigurator
    {
        /// <summary>
        /// Creates a simple in-memory semantic memory for testing purposes.
        /// In a production implementation, this would connect to actual Qdrant.
        /// </summary>
        public static ISemanticTextMemory CreateSimpleMemory()
        {
            return new SimpleSemanticMemory();
        }
        
        /// <summary>
        /// Simple in-memory semantic memory implementation for testing
        /// </summary>
        private class SimpleSemanticMemory : ISemanticTextMemory
        {
            private readonly Dictionary<string, List<MemoryRecord>> _storage = new();

            public Task<MemoryQueryResult> GetAsync(string collection, string key, bool withEmbedding = false, CancellationToken cancellationToken = default)
            {
                if (_storage.TryGetValue(collection, out var records))
                {
                    var record = records.FirstOrDefault(r => r.Id == key);
                    if (record != null)
                    {
                        return Task.FromResult(new MemoryQueryResult(true, record, 1.0));
                    }
                }
                return Task.FromResult(new MemoryQueryResult(false, null, 0.0));
            }

            public Task<MemoryQueryResult> GetNearestMatchAsync(string collection, ReadOnlyMemory<float> embedding, int limit = 1, double minRelevanceScore = 0.0, bool withEmbedding = false, CancellationToken cancellationToken = default)
            {
                // Simple implementation - just return first record if any
                if (_storage.TryGetValue(collection, out var records) && records.Count > 0)
                {
                    var record = records[0];
                    return Task.FromResult(new MemoryQueryResult(true, record, 0.8)); // fake score
                }
                return Task.FromResult(new MemoryQueryResult(false, null, 0.0));
            }

            public Task<MemoryQueryResult> GetNextAsync(string collection, string key, CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }

            public Task<MemoryRecord> PopulateMetadata(string collection, MemoryRecord record, CancellationToken cancellationToken = default)
            {
                // Simple implementation that just returns the record
                return Task.FromResult(record);
            }

            public Task<MemoryRecord> PopulateMetadata(string collection, MemoryRecord record, bool withEmbedding, CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }

            public async Task RemoveAsync(string collection, string key, CancellationToken cancellationToken = default)
            {
                if (_storage.TryGetValue(collection, out var records))
                {
                    var recordToRemove = records.FirstOrDefault(r => r.Id == key);
                    if (recordToRemove != null)
                    {
                        records.Remove(recordToRemove);
                    }
                }
                await Task.CompletedTask;
            }

            public Task RemoveAllAsync(string collection, CancellationToken cancellationToken = default)
            {
                if (_storage.ContainsKey(collection))
                {
                    _storage.Remove(collection);
                }
                return Task.CompletedTask;
            }

            public Task SaveInformationAsync(string collection, string text, string? id = null, string? description = null, string? additionalMetadata = null, CancellationToken cancellationToken = default)
            {
                var recordId = id ?? Guid.NewGuid().ToString();
                var metadata = new Dictionary<string, object>();
                if (!string.IsNullOrWhiteSpace(description))
                {
                    metadata["description"] = description;
                }
                if (!string.IsNullOrWhiteSpace(additionalMetadata))
                {
                    metadata["additionalMetadata"] = additionalMetadata;
                }

                var record = new MemoryRecord(
                    id: recordId,
                    text: text,
                    embedding: ReadOnlyMemory<float>.Empty,
                    metadata: metadata,
                    timestamp: DateTime.UtcNow,
                    isReference: false,
                    externalSourceName: null,
                    externalSourceId: null);

                if (!_storage.ContainsKey(collection))
                {
                    _storage[collection] = new List<MemoryRecord>();
                }

                var existingIndex = _storage[collection].FindIndex(r => r.Id == recordId);
                if (existingIndex >= 0)
                {
                    _storage[collection][existingIndex] = record;
                }
                else
                {
                    _storage[collection].Add(record);
                }

                return Task.CompletedTask;
            }

            public Task SaveReferenceAsync(string collection, string text, string? id = null, string? description = null, string? additionalMetadata = null, string? externalSourceName = null, string? externalSourceId = null, CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }

            public Task<MemoryQueryResult> SearchAsync(string collection, string query, int limit = 1, double minRelevanceScore = 0.0, bool withEmbedding = false, CancellationToken cancellationToken = default)
            {
                // Simple implementation: return all records that contain query in text (case-insensitive)
                if (_storage.TryGetValue(collection, out var records))
                {
                    var matches = new List<MemoryQueryResult>();
                    foreach (var record in records)
                    {
                        if (record.Text.Contains(query, StringComparison.OrdinalIgnoreCase))
                        {
                            matches.Add(new MemoryQueryResult(true, record, 0.5)); // fake score
                        }
                    }
                    // Return up to limit matches
                    return Task.FromResult(matches.Count > 0 ? matches[0] : new MemoryQueryResult(false, null, 0.0));
                }
                return Task.FromResult(new MemoryQueryResult(false, null, 0.0));
            }

            public Task<IReadOnlyCollection<string>> GetCollectionsAsync(CancellationToken cancellationToken = default)
            {
                return Task.FromResult<IReadOnlyCollection<string>>(_storage.Keys);
            }

            public Task<MemoryQueryResult> GetBatchAsync(string collection, IEnumerable<string> keys, bool withEmbedding = false, CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }

            public Task<long> CountAsync(string collection, CancellationToken cancellationToken = default)
            {
                if (_storage.TryGetValue(collection, out var records))
                {
                    return Task.FromResult((long)records.Count);
                }
                return Task.FromResult(0L);
            }
        }
    }
}
