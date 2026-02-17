using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrewAI.DotNet.Core.Interfaces
{
    public interface IKnowledgeSource
    {
        Task<IEnumerable<string>> GetContentChunksAsync();
    }
}
