using System;
using System.Collections.Generic;

namespace AgentsCrew.Core.Knowledge.ChunkingStrategies
{
    public class FixedSizeChunker : IChunkingStrategy
    {
        private readonly int _chunkSize;
        private readonly int _overlap;

        public FixedSizeChunker(int chunkSize = 1000, int overlap = 100)
        {
            if (chunkSize <= 0) throw new ArgumentOutOfRangeException(nameof(chunkSize));
            if (overlap < 0 || overlap >= chunkSize) throw new ArgumentException("Overlap must be non-negative and less than ChunkSize.", nameof(overlap));

            _chunkSize = chunkSize;
            _overlap = overlap;
        }

        public IEnumerable<string> Chunk(string text)
        {
            if (string.IsNullOrEmpty(text))
                yield break;

            int currentIndex = 0;
            int step = _chunkSize - _overlap;

            while (currentIndex < text.Length)
            {
                int length = Math.Min(_chunkSize, text.Length - currentIndex);
                yield return text.Substring(currentIndex, length);
                currentIndex += step;
            }
        }
    }
}
