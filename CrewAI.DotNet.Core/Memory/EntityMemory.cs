using System.Collections.Concurrent;
using CrewAI.DotNet.Core.Interfaces;

namespace CrewAI.DotNet.Core.Memory
{
    public class EntityMemory : IEntityMemory
    {
        private readonly ConcurrentDictionary<string, string> _entities = new();

        public Task AddEntityAsync(string entity, string description)
        {
            _entities.TryAdd(entity, description);
            return Task.CompletedTask;
        }

        public Task<string> GetEntityAsync(string entity)
        {
            _entities.TryGetValue(entity, out var description);
            return Task.FromResult(description ?? string.Empty);
        }
    }
}
