using System.Threading.Tasks;

namespace AgentsCrew.Core.Interfaces
{
    public interface ILongTermMemory
    {
        Task SaveAsync(string key, string content);
        Task<string> SearchAsync(string query);
    }
}
