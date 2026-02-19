# Quickstart Guide: Your First Crew

This guide walks you through creating a simple Crew with two agents: a Researcher and a Writer. They will collaborate to produce a short article.

## 1. Setup Your Project

Ensure you have followed the [Installation Guide](installation.md).

## 2. Define Your Agents

We use the fluent `AgentBuilder` API to define agents.

```csharp
using AgentsCrew.Core.Builders;
using Microsoft.SemanticKernel;

// Initialize Semantic Kernel (replace with real OpenAI setup)
var kernelBuilder = Kernel.CreateBuilder();
// kernelBuilder.AddOpenAIChatCompletion("gpt-4", "YOUR_API_KEY");
var kernel = kernelBuilder.Build();

var researcher = new AgentBuilder()
    .WithRole("Researcher")
    .WithGoal("Uncover groundbreaking technologies")
    .WithBackstory("An expert technology analyst with a keen eye for innovation.")
    .WithKernel(kernel)
    .Build();

var writer = new AgentBuilder()
    .WithRole("Writer")
    .WithGoal("Write compelling tech articles")
    .WithBackstory("A seasoned tech journalist who simplifies complex topics.")
    .WithKernel(kernel)
    .Build();
```

## 3. Define Tasks

Tasks are specific units of work assigned to agents.

```csharp
using AgentsCrew.Core.Builders;

var task1 = new CrewTaskBuilder()
    .WithDescription("Research the latest trends in AI Agents for 2024.")
    .WithExpectedOutput("A bulleted list of top 5 trends.")
    .AssignTo(researcher)
    .Build();

var task2 = new CrewTaskBuilder()
    .WithDescription("Write a blog post based on the research provided.")
    .WithExpectedOutput("A 500-word blog post in markdown format.")
    .AssignTo(writer)
    .Build();
```

## 4. Assemble and Run the Crew

Combine agents and tasks into a Crew and kick off execution.

```csharp
using AgentsCrew.Core.Builders;

var crew = new CrewBuilder()
    .AddAgent(researcher)
    .AddAgent(writer)
    .AddTask(task1)
    .AddTask(task2)
    .Build();

Console.WriteLine("Starting Crew...");
await crew.KickoffAsync();
Console.WriteLine("Crew Finished!");

// Access Results from Memory
var memory = crew.MemoryContext.ShortTerm.Get();
foreach (var item in memory)
{
    Console.WriteLine(item);
}
```

## Understanding the Flow

1.  **Sequential Process:** By default, the Crew executes tasks in the order they are added.
2.  **Context Passing:** The output of `task1` (Researcher) is automatically stored in `ShortTermMemory`.
3.  **Context Usage:** When `task2` (Writer) executes, the Agent receives the contents of Short-Term Memory as context, allowing it to "see" the research results without manual passing.

## Next Steps

- Learn about [Agents](../concepts/agents.md) and [Tasks](../concepts/tasks.md) in detail.
- Explore [Memory Systems](../concepts/memory.md).
- Use [YAML Configuration](../concepts/configuration.md) for cleaner setups.
