# API Reference

This section provides an overview of the key interfaces and classes available in the **CrewAI.DotNet** library.

## Core Interfaces

| Interface | Description |
| :--- | :--- |
| `IAgent` | Defines an autonomous agent with a role, goal, and tools. |
| `ICrew` | Represents a collection of agents and tasks working together. |
| `ICrewTask` | A specific unit of work with a description and expected output. |
| `IProcess` | Orchestrates the execution flow of tasks (Sequential, Hierarchical). |
| `IMemoryContext` | Provides access to Short-Term, Long-Term, and Entity memory. |
| `IAgentManager` | Manages the registry of agents and facilitates delegation. |
| `IEventSystem` | Publishes lifecycle events (start/end/action). |
| `IKnowledgeSource` | Represents a source of knowledge (e.g., text file) for RAG ingestion. |

## Builders (Fluent API)

| Class | Description |
| :--- | :--- |
| `AgentBuilder` | Simplifies creating and configuring `IAgent` instances. |
| `CrewBuilder` | Assembles Agents, Tasks, Process, and Memory into a `Crew`. |
| `CrewTaskBuilder` | Creates `ICrewTask` definitions. |

## Process Implementations

| Class | Description |
| :--- | :--- |
| `SequentialProcess` | Executes tasks one-by-one in the defined order. |
| `HierarchicalProcess` | Assigns tasks to a Manager Agent for dynamic delegation. |

## Tools

| Class | Description |
| :--- | :--- |
| `FileTool` | Provides methods to read/write files (`ReadFileAsync`, `WriteFileAsync`). |
| `WebSearchTool` | Provides web search capability via `IWebSearchService`. |

## Configuration (YAML)

| Class | Description |
| :--- | :--- |
| `YamlConfigurationLoader` | Loads `CrewConfig` (Agents/Tasks) from a YAML file. |
| `AgentConfig` | Represents an agent definition in YAML. |
| `TaskConfig` | Represents a task definition in YAML. |
