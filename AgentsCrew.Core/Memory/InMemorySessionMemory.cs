using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AgentsCrew.Core.Interfaces;

namespace AgentsCrew.Core.Memory
{
    public class InMemorySessionMemory : ISessionMemory
    {
        private readonly ConcurrentDictionary<string, object> _store = new(StringComparer.OrdinalIgnoreCase);

        public string SessionId { get; }

        public InMemorySessionMemory(string sessionId)
        {
            if (string.IsNullOrWhiteSpace(sessionId))
                throw new ArgumentException("SessionId cannot be null or whitespace.", nameof(sessionId));

            SessionId = sessionId;
        }

        public Task SetAsync(string key, object value, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key cannot be null or whitespace.", nameof(key));

            _store[key] = value;
            return Task.CompletedTask;
        }

        public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key cannot be null or whitespace.", nameof(key));

            if (_store.TryGetValue(key, out var value) && value is T typedValue)
            {
                return Task.FromResult<T?>(typedValue);
            }

            return Task.FromResult<T?>(default);
        }

        public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key cannot be null or whitespace.", nameof(key));

            _store.TryRemove(key, out _);
            return Task.CompletedTask;
        }

        public Task ClearAsync(CancellationToken cancellationToken = default)
        {
            _store.Clear();
            return Task.CompletedTask;
        }

        public Task<IEnumerable<string>> GetKeysAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IEnumerable<string>>(_store.Keys.ToList());
        }
    }
}
