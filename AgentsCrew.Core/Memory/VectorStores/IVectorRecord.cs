using System;
using System.Collections.Generic;

namespace AgentsCrew.Core.Memory.VectorStores
{
    public interface IVectorRecord
    {
        string Id { get; set; }
        ReadOnlyMemory<float> Embedding { get; set; }
        string? Content { get; set; }
        IDictionary<string, object>? Metadata { get; set; }
    }
}
