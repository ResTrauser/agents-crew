using System.Linq;
using System.Threading.Tasks;
using CrewAI.DotNet.Core.Interfaces;
using Microsoft.SemanticKernel.Memory;

namespace CrewAI.DotNet.Core.Memory
{
    public class LongTermMemory : ILongTermMemory
    {
        private readonly ISemanticTextMemory _memory;

        public LongTermMemory(ISemanticTextMemory memory)
        {
            _memory = memory;
        }

        public async Task SaveAsync(string key, string content)
        {
            await _memory.SaveInformationAsync("LongTermMemory", content, key);
        }

        public async Task<string> SearchAsync(string query)
        {
            var result = await _memory.SearchAsync("LongTermMemory", query, limit: 1).FirstOrDefaultAsync();
            return result?.Metadata.Text ?? string.Empty;
        }
    }
}
