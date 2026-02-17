# Tasks

Tasks are specific assignments that agents must complete. They define **what** needs to be done.

## Key Attributes

- **Description:** A clear, concise statement of what the task entails.
- **Expected Output:** A description of what the result should look like. This helps the agent understand the format and detail required.
- **Assigned Agent:** The specific agent responsible for the task.
- **Output Type (Structured Output):** Optional. A C# Type definition that the output should conform to (as JSON).

## Task Creation

Tasks are created using the `CrewTaskBuilder` or via YAML.

### Fluent API

```csharp
var task = new CrewTaskBuilder()
    .WithDescription("Analyze the Q3 financial report.")
    .WithExpectedOutput("A summary of key metrics: Revenue, Net Profit, Growth.")
    .AssignTo(analystAgent)
    .Build();
```

### Structured Output

To enforce a specific schema for the output (e.g., for downstream processing), set the `OutputType`.

```csharp
public class FinancialSummary
{
    public decimal Revenue { get; set; }
    public decimal NetProfit { get; set; }
    public string KeyTakeaway { get; set; }
}

task.OutputType = typeof(FinancialSummary);
```

When `OutputType` is set, the Agent automatically appends instructions to the prompt requiring the LLM to return valid JSON matching the type's schema.

### YAML Configuration

```yaml
description: Analyze the Q3 financial report.
expected_output: A summary of key metrics.
assigned_agent: Senior Data Analyst
```
