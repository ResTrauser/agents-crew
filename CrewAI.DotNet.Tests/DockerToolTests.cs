using System.Threading.Tasks;
using Xunit;
using CrewAI.DotNet.Tools;

namespace CrewAI.DotNet.Tests
{
    public class DockerToolTests
    {
        [Fact(Skip = "Requires local docker daemon running")] // Skip on CI or standard runs if docker daemon is absent
        public async Task DockerExecutionTool_Executes_Python_Successfully()
        {
            // Arrange
            var tool = new DockerExecutionTool();
            var pythonCode = "print('Hello from Docker sandbox!')";

            // Act
            var result = await tool.ExecutePythonCodeAsync(pythonCode);

            // Assert
            Assert.Contains("Hello from Docker sandbox!", result);
        }

        [Fact(Skip = "Requires local docker daemon running")] // Skip on CI or standard runs if docker daemon is absent
        public async Task DockerExecutionTool_Executes_Bash_Successfully()
        {
            // Arrange
            var tool = new DockerExecutionTool();
            var bashCommand = "echo 'Bash in Alpine isolated environment'";

            // Act
            var result = await tool.ExecuteBashCommandAsync(bashCommand);

            // Assert
            Assert.Contains("Bash in Alpine isolated environment", result);
        }
    }
}
