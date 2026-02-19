# Tools

Tools (skills/plugins) extend an agent's capabilities beyond text generation. They allow agents to interact with the outside world (API calls, file I/O, search, etc.).

## Semantic Kernel Plugins

In Agents Crew, tools are simply **Semantic Kernel Plugins**. Any class with methods decorated with `[KernelFunction]` can be a tool.

## Standard Toolkit

The library includes a `AgentsCrew.Tools` package with standard tools:

### FileTool
Reads and writes local files.
```csharp
// Usage in Agent
agent.Tools.Add(KernelPluginFactory.CreateFromType<FileTool>());
```

### WebSearchTool
Performs web searches. Requires an implementation of `IWebSearchService`.
```csharp
agent.Tools.Add(KernelPluginFactory.CreateFromObject(new WebSearchTool(mySearchService)));
```

## Creating Custom Tools

1.  Create a class.
2.  Add a public method.
3.  Decorate with `[KernelFunction]` and `[Description]`.

```csharp
public class CalculatorTool
{
    [KernelFunction]
    [Description("Adds two numbers.")]
    public int Add(
        [Description("The first number")] int a,
        [Description("The second number")] int b)
    {
        return a + b;
    }
}
```

## Registering Tools

Add tools to an agent using the `AgentBuilder`:

```csharp
var agent = new AgentBuilder()
    .AddTool(KernelPluginFactory.CreateFromType<CalculatorTool>("Calculator"))
    .Build();
```

When the agent runs, the LLM will see "Calculator-Add" as an available function and can choose to invoke it if the task requires math.
