using System.Threading.Tasks;

namespace CrewAI.DotNet.Core.Interfaces
{
    public interface IEntityMemory
    {
        Task AddEntityAsync(string entity, string description);
        Task<string> GetEntityAsync(string entity);
    }
}
