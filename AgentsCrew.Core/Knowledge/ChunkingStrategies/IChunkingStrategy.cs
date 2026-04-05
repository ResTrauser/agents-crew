using System.Collections.Generic;

namespace AgentsCrew.Core.Knowledge.ChunkingStrategies
{
    public interface IChunkingStrategy
    {
        IEnumerable<string> Chunk(string text);
    }
}
