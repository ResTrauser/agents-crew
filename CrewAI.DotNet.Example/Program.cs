using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CrewAI.DotNet.Core.Builders;
using CrewAI.DotNet.Core.Configuration;
using CrewAI.DotNet.Core.Interfaces;
using CrewAI.DotNet.Core.Process;
using CrewAI.DotNet.Core.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using CrewAI.DotNet.Core.Plugins;
using CrewAI.DotNet.Core.Knowledge;
using CrewAI.DotNet.Tools;
using System.IO;

namespace CrewAI.DotNet.Example
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Initializing CrewAI .NET Example with Dynamic Delegation, Knowledge, and Tools...");

            // Create a Kernel with Mock Chat Completion Service
            var kernelBuilder = Kernel.CreateBuilder();
            kernelBuilder.Services.AddLogging(c => c.AddConsole().SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Warning));
            kernelBuilder.Services.AddSingleton<IChatCompletionService>(new DelegationMockChatCompletionService());
            var kernel = kernelBuilder.Build();

            // Setup infrastructure
            var agentManager = new AgentManager();
#pragma warning disable SKEXP0001
            var semanticMemory = new VolatileSemanticMemory();
            var memoryContext = new MemoryContext(
                new ShortTermMemory(),
                new LongTermMemory(semanticMemory),
                new EntityMemory()
            );
#pragma warning restore SKEXP0001

            // Create a dummy knowledge file
            await File.WriteAllTextAsync("knowledge.txt", "CrewAI is a framework for orchestrating AI agents.");

            // Build Manager Agent
            var manager = new AgentBuilder()
                .WithRole("Manager")
                .WithGoal("Oversee project")
                .WithBackstory("Project Manager")
                .WithKernel(kernel)
                .WithDelegation(agentManager, memoryContext)
                .WithAgentCreation(agentManager, kernel, memoryContext)
                .Build();

            // Add Knowledge Source
            manager.KnowledgeSources.Add(new TextFileKnowledgeSource("knowledge.txt"));

            // Build Developer Agent
            var developer = new AgentBuilder()
                .WithRole("Developer")
                .WithGoal("Write code")
                .WithBackstory("Senior Developer")
                .WithKernel(kernel)
                .Build();

            // Add File Tool
            // developer.Tools.Add(KernelPluginFactory.CreateFromType<FileTool>("FileTool")); // Need to create instance or type

            agentManager.RegisterAgent(manager);
            agentManager.RegisterAgent(developer);

            // Create Task for Manager
            var task = new CrewTaskBuilder()
                .WithDescription("Coordinate the development of the app. Delegate coding tasks to Developer.")
                .WithExpectedOutput("App development complete.")
                .AssignTo(manager)
                .Build();

            // Define output type for structured output test
            task.OutputType = typeof(AppResult);

            // Create Crew
            var crew = new CrewBuilder()
                .AddAgent(manager)
                .AddAgent(developer)
                .AddTask(task)
                .WithMemory(memoryContext)
                .Build();

            // Kickoff
            Console.WriteLine("Starting Crew execution...");
            await crew.KickoffAsync();
            Console.WriteLine("Crew execution completed.");

            // Display results
            var memory = memoryContext.ShortTerm.Get();
            Console.WriteLine("\nShort Term Memory Dump:");
            foreach (var item in memory)
            {
                Console.WriteLine("--------------------------------------------------");
                Console.WriteLine(item);
            }
        }
    }

    public class AppResult
    {
        public string Status { get; set; }
        public string Message { get; set; }
    }

    public class DelegationMockChatCompletionService : IChatCompletionService
    {
        public IReadOnlyDictionary<string, object?> Attributes => new Dictionary<string, object?>();

        public Task<IReadOnlyList<ChatMessageContent>> GetChatMessageContentsAsync(ChatHistory chatHistory, PromptExecutionSettings? executionSettings = null, Kernel? kernel = null, CancellationToken cancellationToken = default)
        {
            var lastMessage = chatHistory.Last().Content;
            string response = "I don't know what to do.";

            // Log for debugging
            // Console.WriteLine($"Mock Service received message from {chatHistory.Last().Role}: {lastMessage}");

            // Check if this is the Manager trying to delegate
            if (lastMessage != null && lastMessage.Contains("Delegate coding tasks to Developer"))
            {
                // Console.WriteLine("Mock: Triggering Delegation to Developer...");
                var args = new KernelArguments
                {
                    { "agentRole", "Developer" },
                    { "taskDescription", "Code the app" }
                };

                var toolCall = new FunctionCallContent("DelegateTask", "Delegation", "call_" + Guid.NewGuid().ToString("N"), args);

                var message = new ChatMessageContent(AuthorRole.Assistant, content: null);
                message.Items.Add(toolCall);

                return Task.FromResult<IReadOnlyList<ChatMessageContent>>(new List<ChatMessageContent>
                {
                    message
                });
            }

            // Developer execution
            if (lastMessage != null && lastMessage.Contains("Code the app"))
            {
                // Console.WriteLine("Mock: Developer coding...");
                response = "App code written successfully.";
            }

            // Manager handling tool result
            var lastMsg = chatHistory.Last();
            if (lastMsg.Role == AuthorRole.Tool || (lastMsg.Content != null && lastMsg.Content.Contains("Task delegated to Developer")))
            {
                 // Console.WriteLine("Mock: Delegation completed successfully.");
                 // Return structured JSON
                 response = "{ \"Status\": \"Success\", \"Message\": \"App is ready.\" }";
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
