using System.IO;
using System.Threading.Tasks;
using AgentsCrew.Core.Interfaces;

namespace AgentsCrew.Core.Storage
{
    public class FileStorageProvider : IStorageProvider
    {
        private readonly string _storageDirectory;

        public FileStorageProvider(string storageDirectory = ".crewai_storage")
        {
            _storageDirectory = storageDirectory;
            if (!Directory.Exists(_storageDirectory))
            {
                Directory.CreateDirectory(_storageDirectory);
            }
        }

        public async Task SaveStateAsync(string key, string data)
        {
            var filePath = Path.Combine(_storageDirectory, $"{key}.json");
            await File.WriteAllTextAsync(filePath, data);
        }

        public async Task<string?> LoadStateAsync(string key)
        {
            var filePath = Path.Combine(_storageDirectory, $"{key}.json");
            if (File.Exists(filePath))
            {
                return await File.ReadAllTextAsync(filePath);
            }
            return null;
        }

        public Task DeleteStateAsync(string key)
        {
            var filePath = Path.Combine(_storageDirectory, $"{key}.json");
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            return Task.CompletedTask;
        }
    }
}
