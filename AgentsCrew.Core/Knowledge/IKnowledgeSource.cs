using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AgentsCrew.Core.Knowledge
{
    public interface IKnowledgeSource
    {
        Task<IEnumerable<string>> GetContentChunksAsync(CancellationToken cancellationToken = default);
    }
}
