using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace AgentsCrew.Tools
{
    public class FileTool
    {
        [KernelFunction]
        [Description("Reads the content of a file.")]
        public async Task<string> ReadFileAsync(
            [Description("The path to the file to read.")] string filePath
        )
        {
            if (!File.Exists(filePath))
            {
                return $"Error: File '{filePath}' not found.";
            }
            return await File.ReadAllTextAsync(filePath);
        }

        [KernelFunction]
        [Description("Writes content to a file.")]
        public async Task WriteFileAsync(
            [Description("The path to the file to write.")] string filePath,
            [Description("The content to write.")] string content
        )
        {
            await File.WriteAllTextAsync(filePath, content);
        }
    }
}
