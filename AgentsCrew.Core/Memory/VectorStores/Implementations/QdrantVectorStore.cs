using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AgentsCrew.Core.Embeddings.Interfaces;
using AgentsCrew.Core.Memory.VectorStores;
using AgentsCrew.Core.Memory.VectorStores.Implementations;
using Microsoft.SemanticKernel.Memory;
using Qdrant.Client;
using Qdrant.Client.Grpc;

namespace AgentsCrew.Core.Memory
{
    /// <summary>
    /// Qdrant Vector Store implementation for AgentsCrew Memory System
    /// </summary>
    public class QdrantVectorStore : IVectorStore
    {
        private readonly QdrantClient _client;
        private readonly IEmbeddingGenerator _embeddingGenerator;

        public QdrantVectorStore(QdrantClient client, IEmbeddingGenerator embeddingGenerator)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _embeddingGenerator = embeddingGenerator ?? throw new ArgumentNullException(nameof(embeddingGenerator));
        }

        public async Task<string> UpsertAsync(string collection, IVectorRecord record, CancellationToken cancellationToken = default)
        {
            if (record == null) throw new ArgumentNullException(nameof(record));
            if (string.IsNullOrWhiteSpace(collection)) throw new ArgumentException("Collection name is required", nameof(collection));
            if (record.Embedding.Length == 0) throw new ArgumentException("Embedding is required", nameof(record.Embedding));

            // Create collection if it doesn't exist
            if (!await CollectionExistsAsync(collection, cancellationToken))
            {
                await CreateCollectionAsync(collection, record.Embedding.Length, cancellationToken);
            }

            // Generate proper ID if not provided
            string id = record.Id;
            if (string.IsNullOrWhiteSpace(id))
            {
                id = Guid.NewGuid().ToString();
            }

            // Upsert point to Qdrant
            var point = new PointStruct
            {
                Id = new PointId { Uuid = id },
                Vectors = record.Embedding.ToArray()
            };

            if (record.Metadata != null)
            {
                foreach (var kvp in record.Metadata)
                {
                    point.Payload[kvp.Key] = kvp.Value?.ToString() ?? string.Empty;
                }
            }

            // Add content to payload if available
            if (!string.IsNullOrWhiteSpace(record.Content))
            {
                point.Payload["content"] = record.Content;
            }

            await _client.UpsertAsync(
                collectionName: collection,
                points: new[] { point },
                cancellationToken: cancellationToken);

            return id;
        }

        public async Task<IReadOnlyList<IVectorRecord>> SearchAsync(string collection, ReadOnlyMemory<float> embedding, int topK = 4, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(collection)) throw new ArgumentException("Collection name is required", nameof(collection));
            if (embedding.Length == 0) throw new ArgumentException("Embedding is required", nameof(embedding));
            if (topK <= 0) throw new ArgumentOutOfRangeException(nameof(topK), "Must be greater than zero");

            if (!await CollectionExistsAsync(collection, cancellationToken))
            {
                return Array.Empty<IVectorRecord>();
            }

            var searchResult = await _client.SearchAsync(
                collectionName: collection,
                vector: embedding.ToArray(),
                limit: (ulong)topK,
                payloadSelector: true,
                cancellationToken: cancellationToken);

            var results = new List<IVectorRecord>();
            foreach (var scoredPoint in searchResult)
            {
                var metadata = scoredPoint.Payload
                    .Where(kvp => kvp.Key != "content")
                    .ToDictionary(kvp => kvp.Key, kvp => (object)kvp.Value);

                var record = new VectorRecord(
                    id: scoredPoint.Id.Uuid,
                    embedding: new ReadOnlyMemory<float>(Array.Empty<float>()),
                    content: scoredPoint.Payload.TryGetValue("content", out var content) ? content.ToString() : null,
                    metadata: metadata.Count > 0 ? metadata : null);

                results.Add(record);
            }

            return results;
        }

        public async Task DeleteAsync(string collection, string id, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(collection)) throw new ArgumentException("Collection name is required", nameof(collection));
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("ID is required", nameof(id));

            await _client.DeleteAsync(
                collection,
                new[] { new PointId { Uuid = id } },
                cancellationToken: cancellationToken);
        }

        public async Task<bool> CollectionExistsAsync(string collection, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(collection)) throw new ArgumentException("Collection name is required", nameof(collection));

            try
            {
                var collections = await _client.ListCollectionsAsync(cancellationToken: cancellationToken);
                return collections.Any(c => c == collection);
            }
            catch
            {
                return false;
            }
        }

        public async Task CreateCollectionAsync(string collection, int dimensions, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(collection)) throw new ArgumentException("Collection name is required", nameof(collection));
            if (dimensions <= 0) throw new ArgumentOutOfRangeException(nameof(dimensions), "Must be greater than zero");

            if (!await CollectionExistsAsync(collection, cancellationToken))
            {
                await _client.CreateCollectionAsync(
                    collectionName: collection,
                    vectorsConfig: new VectorParams { Size = (ulong)dimensions, Distance = Distance.Cosine },
                    cancellationToken: cancellationToken);
            }
        }
    }
}
