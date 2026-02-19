using System.Collections.Generic;
using System.Threading.Tasks;

namespace AgentsCrew.Core.Interfaces
{
    public interface IKnowledgeSource
    {
        Task<IEnumerable<string>> GetContentChunksAsync();
    }
}
