using System;
using System.Collections.Generic;

namespace AgentsCrew.Core.Memory.VectorStores
{
    public class VectorRecord : IVectorRecord
    {
        public string Id { get; set; } = string.Empty;
        public ReadOnlyMemory<float> Embedding { get; set; }
        public string? Content { get; set; }
        public IDictionary<string, object>? Metadata { get; set; }

        public VectorRecord()
        {
        }

        public VectorRecord(string id, ReadOnlyMemory<float> embedding, string? content = null, IDictionary<string, object>? metadata = null)
        {
            Id = id;
            Embedding = embedding;
            Content = content;
            Metadata = metadata;
        }
    }
}
