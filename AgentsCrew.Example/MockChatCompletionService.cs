using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace AgentsCrew.Example
{
    public class MockChatCompletionService : IChatCompletionService
    {
        public IReadOnlyDictionary<string, object?> Attributes => new Dictionary<string, object?>();

        public Task<IReadOnlyList<ChatMessageContent>> GetChatMessageContentsAsync(ChatHistory chatHistory, PromptExecutionSettings? executionSettings = null, Kernel? kernel = null, CancellationToken cancellationToken = default)
        {
            // Simple mock logic
            var lastMessage = chatHistory[chatHistory.Count - 1].Content;
            string response = "I am an AI agent.";

            if (lastMessage != null)
            {
                if (lastMessage.Contains("Researcher"))
                {
                    response = "Based on my research, CrewAI is a framework for orchestrating role-playing AI agents. It enables agents to work together to solve complex tasks.";
                }
                else if (lastMessage.Contains("Writer"))
                {
                    response = "Title: The Power of CrewAI\n\nCrewAI revolutionizes AI orchestration by enabling multiple agents to collaborate. This allows for complex workflows where agents like Researchers and Writers work together seamlessly.";
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
