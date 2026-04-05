using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AgentsCrew.Core.Builders;
using AgentsCrew.Core.Interfaces;
using AgentsCrew.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Xunit;

namespace AgentsCrew.Tests
{
    public class NewFeaturesTests
    {
        [Fact]
        public async Task Task_OutputFile_ShouldBeCreated()
        {
            // Arrange
            var kernelBuilder = Kernel.CreateBuilder();
            kernelBuilder.Services.AddSingleton<IChatCompletionService>(new TestMockChatCompletionService());
            var kernel = kernelBuilder.Build();

            var agent = new AgentBuilder()
                .WithRole("Tester")
                .WithGoal("Test OutputFile")
                .WithBackstory("Testing.")
                .WithKernel(kernel)
                .Build();

            string outputFile = "test_output.txt";
            if (File.Exists(outputFile)) File.Delete(outputFile);

            var task = new CrewTaskBuilder()
                .WithDescription("Generate output")
                .WithExpectedOutput("Output content")
                .WithOutputFile(outputFile)
                .AssignTo(agent)
                .Build();

            // Act
            await agent.ExecuteAsync(task);

            // Assert
            Assert.True(File.Exists(outputFile));
            string content = File.ReadAllText(outputFile);
            Assert.Contains("Output content", content);

            // Cleanup
            File.Delete(outputFile);
        }

        [Fact]
        public async Task Task_Callback_ShouldBeInvoked()
        {
            // Arrange
            var kernelBuilder = Kernel.CreateBuilder();
            kernelBuilder.Services.AddSingleton<IChatCompletionService>(new TestMockChatCompletionService());
            var kernel = kernelBuilder.Build();

            var agent = new AgentBuilder()
                .WithRole("Tester")
                .WithGoal("Test Callback")
                .WithBackstory("Testing.")
                .WithKernel(kernel)
                .Build();

            bool callbackInvoked = false;
            string callbackResult = "";

            var task = new CrewTaskBuilder()
                .WithDescription("Generate output")
                .WithExpectedOutput("Output content")
                .WithCallback((result) => {
                    callbackInvoked = true;
                    callbackResult = result;
                })
                .AssignTo(agent)
                .Build();

            // Act
            await agent.ExecuteAsync(task);

            // Assert
            Assert.True(callbackInvoked);
            Assert.Contains("Output content", callbackResult);
        }

        [Fact]
        public async Task Task_Context_ShouldBeIncludedInPrompt()
        {
            // Arrange
            var mockService = new TestMockChatCompletionService();
            var kernelBuilder = Kernel.CreateBuilder();
            kernelBuilder.Services.AddSingleton<IChatCompletionService>(mockService);
            var kernel = kernelBuilder.Build();

            var agent = new AgentBuilder()
                .WithRole("Tester")
                .WithGoal("Test Context")
                .WithBackstory("Testing.")
                .WithKernel(kernel)
                .Build();

            var previousTask = new CrewTaskBuilder()
                .WithDescription("Previous Task")
                .WithExpectedOutput("Previous Output")
                .Build();

            // Simulate execution of previous task
            ((CrewTask)previousTask).Output = "Previous Output Content";

            var task = new CrewTaskBuilder()
                .WithDescription("Current Task")
                .WithExpectedOutput("Current Output")
                .WithContext(new List<ICrewTask> { previousTask })
                .AssignTo(agent)
                .Build();

            // Act
            await agent.ExecuteAsync(task);

            // Assert
            Assert.NotNull(mockService.LastPrompt);
            Assert.Contains("Previous Tasks Context:", mockService.LastPrompt);
            Assert.Contains("Previous Output Content", mockService.LastPrompt);
        }

        private class TestMockChatCompletionService : IChatCompletionService
        {
            public string? LastPrompt { get; private set; }
            public IReadOnlyDictionary<string, object?> Attributes => new Dictionary<string, object?>();

            public Task<IReadOnlyList<ChatMessageContent>> GetChatMessageContentsAsync(ChatHistory chatHistory, PromptExecutionSettings? executionSettings = null, Kernel? kernel = null, CancellationToken cancellationToken = default)
            {
                LastPrompt = chatHistory.Last().Content;
                string response = "Output content";

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
