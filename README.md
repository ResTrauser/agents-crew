# Agents Crew

**Agents Crew** is a powerful framework for orchestrating role-playing autonomous AI agents in .NET. It is inspired by the Python [CrewAI](https://github.com/joaomdmoura/crewAI) library and built on top of [Microsoft Semantic Kernel](https://github.com/microsoft/semantic-kernel).

## Features

- **Autonomous Agents:** Define agents with specific roles, goals, and backstories.
- **Task Orchestration:** Sequence tasks and delegate them dynamically.
- **Hierarchical Processes:** Support for manager agents that delegate work to others.
- **Memory Systems:** Short-term (context), Long-term (vector/RAG), and Entity memory.
- **Tools:** Use Semantic Kernel plugins as tools (File I/O, Web Search).
- **Configuration:** Define agents and tasks via YAML or a fluent C# API.
- **Observability:** Event-driven architecture for tracking execution flow.

## Documentation

Comprehensive documentation is available in the [`docs/`](docs/) directory:

- [**Getting Started**](docs/getting-started/installation.md)
  - [Installation](docs/getting-started/installation.md)
  - [Quickstart Guide](docs/getting-started/quickstart.md)
- [**Core Concepts**](docs/concepts/agents.md)
  - [Agents](docs/concepts/agents.md)
  - [Tasks](docs/concepts/tasks.md)
  - [Crews & Processes](docs/concepts/crews.md)
  - [Memory](docs/concepts/memory.md)
  - [Tools](docs/concepts/tools.md)
- [**Architecture**](docs/architecture/overview.md)
  - [Overview](docs/architecture/overview.md)
  - [Design Decisions](docs/architecture/design-decisions.md)
- [**API Reference**](docs/api-reference/README.md)
- [**Contributing**](docs/CONTRIBUTING.md)

## Example

```csharp
var researcher = new AgentBuilder()
    .WithRole("Researcher")
    .WithGoal("Find new AI trends")
    .Build();

var writer = new AgentBuilder()
    .WithRole("Writer")
    .WithGoal("Write a blog post")
    .Build();

var task1 = new CrewTaskBuilder()
    .WithDescription("Research AI agents.")
    .AssignTo(researcher)
    .Build();

var crew = new CrewBuilder()
    .AddAgent(researcher)
    .AddAgent(writer)
    .AddTask(task1)
    .Build();

await crew.KickoffAsync();
```

## License

MIT License. See [LICENSE](LICENSE) for details.
