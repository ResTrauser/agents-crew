using CrewAI.DotNet.Core.Interfaces;
using Microsoft.SemanticKernel.Memory;

namespace CrewAI.DotNet.Core.Memory
{
    public class LongTermMemory(ISemanticTextMemory memory) : ILongTermMemory
    {
        private readonly ISemanticTextMemory _memory = memory;

        public async Task SaveAsync(string key, string content)
        {
            await _memory.SaveInformationAsync("LongTermMemory", content, key);
        }

        public async Task<string> SearchAsync(string query)
        {
            await foreach (var result in _memory.SearchAsync("LongTermMemory", query, limit: 1))
            {
                return result.Metadata.Text ?? string.Empty;
            }
            return string.Empty;
        }
    }
}
