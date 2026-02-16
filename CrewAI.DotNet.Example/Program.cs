using System;
using System.Threading.Tasks;
using CrewAI.DotNet.Core.Builders;
using CrewAI.DotNet.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace CrewAI.DotNet.Example
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Initializing CrewAI .NET Example...");

            // Create a Kernel with Mock Chat Completion Service
            var kernelBuilder = Kernel.CreateBuilder();
            kernelBuilder.Services.AddSingleton<IChatCompletionService>(new MockChatCompletionService());
            var kernel = kernelBuilder.Build();

            // Create Agents
            var researcher = new AgentBuilder()
                .WithRole("Researcher")
                .WithGoal("Conduct research on CrewAI")
                .WithBackstory("An expert researcher in AI frameworks.")
                .WithKernel(kernel)
                .Build();

            var writer = new AgentBuilder()
                .WithRole("Writer")
                .WithGoal("Write an article about CrewAI")
                .WithBackstory("A skilled tech writer.")
                .WithKernel(kernel)
                .Build();

            // Create Tasks
            var task1 = new CrewTaskBuilder()
                .WithDescription("Research the core concepts of CrewAI.")
                .WithExpectedOutput("A summary of CrewAI concepts.")
                .AssignTo(researcher)
                .Build();

            var task2 = new CrewTaskBuilder()
                .WithDescription("Write a blog post based on the research.")
                .WithExpectedOutput("A 500-word blog post.")
                .AssignTo(writer)
                .Build();

            // Create Crew
            var crew = new CrewBuilder()
                .AddAgent(researcher)
                .AddAgent(writer)
                .AddTask(task1)
                .AddTask(task2)
                .Build();

            // Kickoff
            Console.WriteLine("Starting Crew execution...");
            await crew.KickoffAsync();
            Console.WriteLine("Crew execution completed.");

            // Display results (In a real app, we'd capture results or use callbacks, or check memory)
            // For now, let's verify memory
            var memory = crew.MemoryContext.ShortTerm.Get();
            Console.WriteLine("\nShort Term Memory Dump:");
            foreach (var item in memory)
            {
                Console.WriteLine("--------------------------------------------------");
                Console.WriteLine(item);
            }
        }
    }
}
