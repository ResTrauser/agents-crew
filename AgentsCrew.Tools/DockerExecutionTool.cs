using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace AgentsCrew.Tools
{
    public class DockerExecutionTool
    {
        private readonly DockerClient _client;

        public DockerExecutionTool()
        {
            _client = new DockerClientConfiguration().CreateClient();
        }

        [KernelFunction]
        [Description("Executes Python code in an isolated Docker container and returns the standard output. Useful for sandboxed execution of generated scripts.")]
        public async Task<string> ExecutePythonCodeAsync(
            [Description("The exact Python code script to execute.")] string pythonCode)
        {
            try
            {
                var image = "python:3.9-slim";
                
                // Ensure image is downloaded
                await _client.Images.CreateImageAsync(
                    new ImagesCreateParameters { FromImage = image }, 
                    new AuthConfig(), 
                    new Progress<JSONMessage>());

                // Create container to run Python command
                var response = await _client.Containers.CreateContainerAsync(new CreateContainerParameters
                {
                    Image = image,
                    Cmd = new[] { "python", "-c", pythonCode },
                    Tty = false,
                    AttachStdout = true,
                    AttachStderr = true,
                    HostConfig = new HostConfig
                    {
                        AutoRemove = true // Ensure container is cleaned up afterwards
                    }
                });

                var containerId = response.ID;

                // Start execution
                await _client.Containers.StartContainerAsync(containerId, new ContainerStartParameters());

                // Get logs multiplexed stream
                var logStream = await _client.Containers.GetContainerLogsAsync(containerId, false, new ContainerLogsParameters
                {
                    ShowStdout = true,
                    ShowStderr = true,
                    Follow = true
                });
                
                var (stdout, stderr) = await logStream.ReadOutputToEndAsync(CancellationToken.None);

                var output = stdout?.Trim();
                if (!string.IsNullOrEmpty(stderr))
                {
                    output += "\nErrors:\n" + stderr.Trim();
                }

                return string.IsNullOrEmpty(output) ? "Code executed successfully with no output." : output;
            }
            catch (Exception ex)
            {
                return $"Docker Execution Error: {ex.Message}";
            }
        }
        
        [KernelFunction]
        [Description("Executes a Bash command in an isolated Docker container and returns the standard output.")]
        public async Task<string> ExecuteBashCommandAsync(
            [Description("The exact bash command to execute.")] string bashCommand)
        {
             try
            {
                var image = "alpine:latest";
                
                // Ensure image is downloaded
                await _client.Images.CreateImageAsync(
                    new ImagesCreateParameters { FromImage = image }, 
                    new AuthConfig(), 
                    new Progress<JSONMessage>());

                // Create container to run Bash command
                var response = await _client.Containers.CreateContainerAsync(new CreateContainerParameters
                {
                    Image = image,
                    Cmd = new[] { "sh", "-c", bashCommand },
                    Tty = false,
                    AttachStdout = true,
                    AttachStderr = true,
                    HostConfig = new HostConfig
                    {
                        AutoRemove = true // Ensure container is cleaned up afterwards
                    }
                });

                var containerId = response.ID;

                // Start execution
                await _client.Containers.StartContainerAsync(containerId, new ContainerStartParameters());

                // Get logs multiplexed stream
                var logStream = await _client.Containers.GetContainerLogsAsync(containerId, false, new ContainerLogsParameters
                {
                    ShowStdout = true,
                    ShowStderr = true,
                    Follow = true
                });
                
                var (stdout, stderr) = await logStream.ReadOutputToEndAsync(CancellationToken.None);

                var output = stdout?.Trim();
                if (!string.IsNullOrEmpty(stderr))
                {
                    output += "\nErrors:\n" + stderr.Trim();
                }

                return string.IsNullOrEmpty(output) ? "Command executed successfully with no output." : output;
            }
            catch (Exception ex)
            {
                return $"Docker Execution Error: {ex.Message}";
            }
        }
    }
}
