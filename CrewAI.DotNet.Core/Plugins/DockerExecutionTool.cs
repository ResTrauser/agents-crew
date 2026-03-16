using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.Extensions.Logging;

namespace CrewAI.DotNet.Core.Plugins
{
    /// <summary>
    /// Executes scripts or commands safely inside an ephemeral Docker container.
    /// Acts as a sandbox to prevent Agents from executing malicious code directly on the host.
    /// </summary>
    public class DockerExecutionTool
    {
        private readonly ILogger<DockerExecutionTool>? _logger;
        private readonly string _defaultImage;

        public DockerExecutionTool(string defaultImage = "python:3.11-slim", ILogger<DockerExecutionTool>? logger = null)
        {
            _defaultImage = defaultImage;
            _logger = logger;
        }

        [KernelFunction("ExecutePythonScript")]
        [System.ComponentModel.Description("Executes a Python script securely in a sandboxed Docker container and returns the output.")]
        public async Task<string> ExecutePythonScriptAsync(
            [System.ComponentModel.Description("The raw Python code string to execute.")] string code)
        {
            _logger?.LogInformation("Executing sandboxed Python code...");
            
            var tempFile = Path.GetTempFileName() + ".py";
            await File.WriteAllTextAsync(tempFile, code);

            try
            {
                // Mount the script as read-only, limit memory/cpu for safety, and remove container after run
                var args = $"run --rm --memory=\"256m\" --cpus=\"0.5\" --network none -v \"{tempFile}:/app/script.py:ro\" {_defaultImage} python /app/script.py";
                return await RunProcessAsync("docker", args);
            }
            finally
            {
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }
            }
        }

        [KernelFunction("ExecuteBashCommand")]
        [System.ComponentModel.Description("Executes a bash command securely in a sandboxed Docker container and returns the output.")]
        public async Task<string> ExecuteBashCommandAsync(
            [System.ComponentModel.Description("The bash command to execute.")] string command)
        {
            _logger?.LogInformation("Executing sandboxed Bash command...");
            
            // Encode the command to avoid escaping issues in CLI
            var base64Command = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(command));
            
            // Limit memory, cpu, network, and remove container after run. Use alpine for bash.
            var args = $"run --rm --memory=\"128m\" --cpus=\"0.5\" --network none alpine sh -c \"echo '{base64Command}' | base64 -d | sh\"";
            
            return await RunProcessAsync("docker", args);
        }

        private async Task<string> RunProcessAsync(string fileName, string arguments)
        {
            var processStartInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new System.Diagnostics.Process { StartInfo = processStartInfo };
            
            process.Start();

            var outputTask = process.StandardOutput.ReadToEndAsync();
            var errorTask = process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync();

            var output = await outputTask;
            var error = await errorTask;

            if (process.ExitCode != 0)
            {
                var errorMsg = $"Execution failed with code {process.ExitCode}. Error: {error}";
                _logger?.LogWarning(errorMsg);
                return $"Error: {errorMsg}\nOutput: {output}";
            }

            return output;
        }
    }
}
