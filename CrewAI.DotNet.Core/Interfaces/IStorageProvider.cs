using System.Threading.Tasks;

namespace CrewAI.DotNet.Core.Interfaces
{
    public interface IStorageProvider
    {
        Task SaveStateAsync(string key, string data);
        Task<string?> LoadStateAsync(string key);
        Task DeleteStateAsync(string key);
    }
}
