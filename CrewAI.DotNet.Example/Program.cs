using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrewAI.DotNet.Core.Builders;
using CrewAI.DotNet.Core.Configuration;
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
            Console.WriteLine("Initializing CrewAI .NET Example with YAML Config...");

            // Create a Kernel with Mock Chat Completion Service
            var kernelBuilder = Kernel.CreateBuilder();
            kernelBuilder.Services.AddSingleton<IChatCompletionService>(new MockChatCompletionService());
            var kernel = kernelBuilder.Build();

            // Load Configuration
            var loader = new YamlConfigurationLoader();
            var config = loader.LoadFromFile<CrewConfig>("crew_config.yaml");

            // Build Agents from Config
            var agents = new Dictionary<string, IAgent>();
            foreach (var agentConfig in config.Agents)
            {
                var agent = new AgentBuilder()
                    .FromConfig(agentConfig)
                    .WithKernel(kernel)
                    .Build();

                // Assuming Role is unique for this example
                agents[agentConfig.Role] = agent;
                Console.WriteLine($"Agent created: {agent.Role}");
            }

            // Build Tasks from Config
            var tasks = new List<ICrewTask>();
            foreach (var taskConfig in config.Tasks)
            {
                var taskBuilder = new CrewTaskBuilder()
                    .FromConfig(taskConfig);

                if (!string.IsNullOrEmpty(taskConfig.AssignedAgent) && agents.ContainsKey(taskConfig.AssignedAgent))
                {
                    taskBuilder.AssignTo(agents[taskConfig.AssignedAgent]);
                }

                tasks.Add(taskBuilder.Build());
                Console.WriteLine($"Task created: {taskConfig.Description}");
            }

            // Create Crew
            var crewBuilder = new CrewBuilder();
            foreach (var agent in agents.Values)
            {
                crewBuilder.AddAgent(agent);
            }
            foreach (var task in tasks)
            {
                crewBuilder.AddTask(task);
            }

            var crew = crewBuilder.Build();

            // Kickoff
            Console.WriteLine("Starting Crew execution...");
            await crew.KickoffAsync();
            Console.WriteLine("Crew execution completed.");

            // Display results
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
