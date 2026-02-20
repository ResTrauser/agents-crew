using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CrewAI.DotNet.Core.Interfaces;
using CrewAI.DotNet.Core.Models;
using CrewAI.DotNet.Core.Builders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Xunit;
using System.Linq;
using System.Threading;

namespace CrewAI.DotNet.Tests
{
    public class SemanticKernelInteropTests
    {
        [Fact]
        public async Task Crew_ShouldOrchestrate_SemanticKernelWrapperAgent()
        {
            // Arrange
            // 1. Setup a Mock Kernel
            var kernelBuilder = Kernel.CreateBuilder();
            kernelBuilder.Services.AddSingleton<IChatCompletionService>(new MockSKChatCompletionService());
            var kernel = kernelBuilder.Build();

            // 2. Define the custom agent using the library's SemanticKernelWrapperAgent
            // The prompt expects {{ $input }}
            string skPrompt = "You are a specialized bot. Process this input: {{ $input }}";
            // Uses the wrapper available in Core.Models
            var customAgent = new SemanticKernelWrapperAgent(kernel, skPrompt);

            // 3. Define a task for this agent
            var task = new CrewTaskBuilder()
                .WithDescription("Hello World")
                .WithExpectedOutput("Processed: Hello World")
                .AssignTo(customAgent)
                .Build();

            // 4. Create the Crew
            var crew = new CrewBuilder()
                .AddAgent(customAgent)
                .AddTask(task)
                .Build();

            // Act
            await crew.KickoffAsync();

            // Assert
            Assert.NotNull(task.Output);
            Assert.Contains("Processed: Hello World", task.Output);
        }

        // Mock Chat Service tailored for this test
        private class MockSKChatCompletionService : IChatCompletionService
        {
            public IReadOnlyDictionary<string, object?> Attributes => new Dictionary<string, object?>();

            public Task<IReadOnlyList<ChatMessageContent>> GetChatMessageContentsAsync(ChatHistory chatHistory, PromptExecutionSettings? executionSettings = null, Kernel? kernel = null, CancellationToken cancellationToken = default)
            {
                // Inspect the chat history to see what prompt was sent.
                // The prompt template is: "You are a specialized bot. Process this input: {{ $input }}"
                // If input is "Hello World", the prompt sent to LLM should contain "Process this input: Hello World"

                var userMessage = chatHistory.LastOrDefault()?.Content ?? "";
                string response = "I don't know what to do.";

                if (userMessage.Contains("Process this input: Hello World"))
                {
                    response = "Processed: Hello World";
                }

                return Task.FromResult<IReadOnlyList<ChatMessageContent>>(new List<ChatMessageContent>
                {
                    new ChatMessageContent(AuthorRole.Assistant, response)
                });
            }

            public IAsyncEnumerable<StreamingChatMessageContent> GetStreamingChatMessageContentsAsync(ChatHistory chatHistory, PromptExecutionSettings? executionSettings = null, Kernel? kernel = null, CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }
        }
    }
}
