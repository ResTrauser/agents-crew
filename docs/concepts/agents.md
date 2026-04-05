# Agents

Agents are the core building blocks of CrewAI. An agent is an autonomous unit programmed to:
1.  Perform tasks.
2.  Make decisions.
3.  Use tools.

## Key Attributes

- **Role:** The function the agent performs (e.g., "Researcher", "Content Writer").
- **Goal:** The objective the agent aims to achieve. This guides the agent's decision-making process.
- **Backstory:** Provides context to the agent's persona, influencing how it interacts and responds.
- **Tools:** Capabilities that an agent can use to perform actions (e.g., search the web, read a file).
- **Knowledge Sources:** Documents or data sources the agent can reference.

## Agent Creation

You can create agents using the `AgentBuilder` fluent API or via YAML configuration.

### Fluent API

```csharp
var agent = new AgentBuilder()
    .WithRole("Senior Data Analyst")
    .WithGoal("Uncover insights in financial data")
    .WithBackstory("Seasoned financial analyst with 10 years of experience.")
    .WithKernel(kernel)
    .AddTool(new MyCustomTool())
    .Build();
```

### YAML Configuration

```yaml
role: Senior Data Analyst
goal: Uncover insights in financial data
backstory: Seasoned financial analyst with 10 years of experience.
```

## Agent Execution

When an agent executes a task (`ExecuteAsync`), it:
1.  **Constructs Context:** Retrieves relevant information from Short-Term, Long-Term, and Entity Memory.
2.  **Ingests Knowledge:** If `KnowledgeSources` are configured, it ingests them into Long-Term Memory (RAG).
3.  **Constructs Prompt:** Creates a comprehensive prompt including Role, Goal, Task, and Context.
4.  **Invokes LLM:** Uses the Semantic Kernel to process the prompt.
5.  **Uses Tools:** If the LLM decides to call a tool (e.g., `SearchAsync`), the agent executes it and feeds the result back to the LLM.
6.  **Returns Output:** The final response is returned and stored in Short-Term Memory.

## Thread Safety

Each agent execution runs within a **Scoped Kernel**. This means the base `Kernel` is cloned for the duration of the task. This ensures that tool execution state and plugins do not leak or conflict between concurrent agent executions.
