using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AgentsCrew.Core.Interfaces
{
    public interface ISessionMemory
    {
        string SessionId { get; }
        
        Task SetAsync(string key, object value, CancellationToken cancellationToken = default);
        Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);
        Task RemoveAsync(string key, CancellationToken cancellationToken = default);
        Task ClearAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<string>> GetKeysAsync(CancellationToken cancellationToken = default);
    }
}
