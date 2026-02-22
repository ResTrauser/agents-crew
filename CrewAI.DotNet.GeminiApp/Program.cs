using System;
using System.Threading.Tasks;
using CrewAI.DotNet.Core.Builders;
using CrewAI.DotNet.Core.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using CrewAI.DotNet.Core.Process;

namespace CrewAI.DotNet.GeminiApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Initializing CrewAI .NET with Google Gemini...");

            var apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY");
            if (string.IsNullOrEmpty(apiKey))
            {
                Console.WriteLine("Error: Please set the GEMINI_API_KEY environment variable.");
                return;
            }

            // Create a Kernel with Gemini
            string modelId = Environment.GetEnvironmentVariable("GEMINI_MODEL") ?? "gemini-2.5-flash";
            Console.WriteLine($"Using model: {modelId}");
            
            var kernelBuilder = Kernel.CreateBuilder();
            kernelBuilder.Services.AddLogging(c => c.AddConsole().SetMinimumLevel(LogLevel.Warning));
            kernelBuilder.AddGoogleAIGeminiChatCompletion(modelId: modelId, apiKey: apiKey);
            var kernel = kernelBuilder.Build();

            // Build Researcher Agent
            var researcher = new AgentBuilder()
                .WithRole("Tech Analyst")
                .WithGoal("Analyze the latest trends in AI agents")
                .WithBackstory("An expert analyst following emerging AI paradigms.")
                .WithKernel(kernel)
                .Build();

            // Build Writer Agent
            var writer = new AgentBuilder()
                .WithRole("Tech Writer")
                .WithGoal("Write a compelling short blog post based on research findings")
                .WithBackstory("A skilled tech writer who turns complex trends into engaging stories.")
                .WithKernel(kernel)
                .Build();


            // Setup Memory Context
#pragma warning disable SKEXP0001
            var memoryContext = new MemoryContext(
                new ShortTermMemory(),
                new LongTermMemory(new VolatileSemanticMemory()),
                new EntityMemory()
            );
#pragma warning restore SKEXP0001

            // Setup Agent Manager and Manager Agent (Orchestrator)
            var agentManager = new AgentManager(new[] { researcher, writer });
            
            var manager = new AgentBuilder()
                .WithRole("Orquestador o Manager")
                .WithGoal("Manage the crew and delegate tasks to the appropriate agents based on their roles.")
                .WithBackstory("An experienced project manager skilled at analyzing requirements and delegating work to the best suited team members.")
                .WithKernel(kernel)
                .WithDelegation(agentManager, memoryContext)
                .Build();

            // Create Research Task (Unassigned, manager will delegate)
            var researchTask = new CrewTaskBuilder()
                .WithDescription("Research the current state and future predictions of multi-agent LLM systems. Summarize key points.")
                .WithExpectedOutput("A list of 3-5 key trends regarding multi-agent LLM systems.")
                .Build();

            // Create Writing Task (Unassigned, manager will delegate)
            var writeTask = new CrewTaskBuilder()
                .WithDescription("Using the research findings, write a 2-paragraph blog post about multi-agent systems focusing on practical applications.")
                .WithExpectedOutput("A 2-paragraph blog post ready for publishing.")
                .Build();



            // Create Crew with Hierarchical Process
            var crew = new CrewBuilder()
                .AddAgent(researcher)
                .AddAgent(writer)
                .AddAgent(manager)
                .AddTask(researchTask)
                .AddTask(writeTask)
                .WithProcess(new HierarchicalProcess(manager))
                .WithMemory(memoryContext)
                .Build();

            // Kickoff
            Console.WriteLine("\nStarting Crew execution...");
            await crew.KickoffAsync();
            Console.WriteLine("Crew execution completed.\n");

            // Display results
            Console.WriteLine("=== Final Result (Short Term Memory) ===");
            var memory = memoryContext.ShortTerm.Get();
            foreach (var item in memory)
            {
                Console.WriteLine("--------------------------------------------------");
                Console.WriteLine(item);
            }
        }
    }
}
