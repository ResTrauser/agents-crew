using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AgentsCrew.Core.Knowledge.ChunkingStrategies
{
    public class SentenceChunker : IChunkingStrategy
    {
        public IEnumerable<string> Chunk(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                yield break;

            // Divide por puntos, signos de exclamación o de interrogación que terminen una oración
            var sentences = Regex.Split(text, @"(?<=[\.!\?])\s+");

            foreach (var sentence in sentences)
            {
                var trimmed = sentence.Trim();
                if (!string.IsNullOrWhiteSpace(trimmed))
                {
                    yield return trimmed;
                }
            }
        }
    }
}
