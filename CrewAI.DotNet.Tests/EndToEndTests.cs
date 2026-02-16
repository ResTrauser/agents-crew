using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CrewAI.DotNet.Core.Builders;
using CrewAI.DotNet.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Xunit;

namespace CrewAI.DotNet.Tests
{
    public class EndToEndTests
    {
        [Fact]
        public async Task CrewExecution_WithMockLLM_ShouldCompleteTasks()
        {
            // Arrange
            var kernelBuilder = Kernel.CreateBuilder();
            kernelBuilder.Services.AddSingleton<IChatCompletionService>(new TestMockChatCompletionService());
            var kernel = kernelBuilder.Build();

            var researcher = new AgentBuilder()
                .WithRole("Researcher")
                .WithGoal("Conduct research")
                .WithBackstory("An expert researcher.")
                .WithKernel(kernel)
                .Build();

            var writer = new AgentBuilder()
                .WithRole("Writer")
                .WithGoal("Write a report")
                .WithBackstory("A skilled writer.")
                .WithKernel(kernel)
                .Build();

            var task1 = new CrewTaskBuilder()
                .WithDescription("Research about AI")
                .WithExpectedOutput("Research summary")
                .AssignTo(researcher)
                .Build();

            var task2 = new CrewTaskBuilder()
                .WithDescription("Write a report")
                .WithExpectedOutput("Report content")
                .AssignTo(writer)
                .Build();

            var crew = new CrewBuilder()
                .AddAgent(researcher)
                .AddAgent(writer)
                .AddTask(task1)
                .AddTask(task2)
                .Build();

            // Act
            await crew.KickoffAsync();

            // Assert
            var memory = crew.MemoryContext.ShortTerm.Get().ToList();
            Assert.Equal(2, memory.Count);
            Assert.Contains(memory, m => m.Contains("Research summary"));
            Assert.Contains(memory, m => m.Contains("Report content"));
        }

        private class TestMockChatCompletionService : IChatCompletionService
        {
            public IReadOnlyDictionary<string, object?> Attributes => new Dictionary<string, object?>();

            public Task<IReadOnlyList<ChatMessageContent>> GetChatMessageContentsAsync(ChatHistory chatHistory, PromptExecutionSettings? executionSettings = null, Kernel? kernel = null, CancellationToken cancellationToken = default)
            {
                var lastMessage = chatHistory.Last().Content;
                string response = "Mock response";

                if (lastMessage != null)
                {
                    if (lastMessage.Contains("Researcher"))
                    {
                        response = "Research summary: AI is great.";
                    }
                    else if (lastMessage.Contains("Writer"))
                    {
                        response = "Report content: AI is changing everything.";
                    }
                }

                return Task.FromResult<IReadOnlyList<ChatMessageContent>>(new List<ChatMessageContent>
                {
                    new ChatMessageContent(AuthorRole.Assistant, response)
                });
            }

            public IAsyncEnumerable<StreamingChatMessageContent> GetStreamingChatMessageContentsAsync(ChatHistory chatHistory, PromptExecutionSettings? executionSettings = null, Kernel? kernel = null, CancellationToken cancellationToken = default)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
