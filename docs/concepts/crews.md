# Crews & Processes

A **Crew** represents a collaborative group of agents working together on a set of tasks. The **Process** defines how these tasks are executed.

## Crew Assembly

A Crew is composed of:
1.  **Agents:** The team members.
2.  **Tasks:** The work to be done.
3.  **Process:** The orchestration strategy.
4.  **Memory:** Shared memory context for the crew.
5.  **Manager (Optional):** An agent designated to oversee execution (for hierarchical processes).

## Process Types

The framework supports two primary process types:

### 1. Sequential Process (Default)

Tasks are executed one after another in the order they are defined in the list.
- **Flow:** Task 1 -> Result -> Context for Task 2 -> Task 2 -> Result ...
- **Use Case:** Linear workflows where each step depends on the previous one (e.g., Research -> Write -> Edit).

```csharp
var crew = new CrewBuilder()
    .WithProcess(new SequentialProcess()) // Default
    // ...
    .Build();
```

### 2. Hierarchical Process

Tasks are assigned to a **Manager Agent**. The Manager evaluates the tasks and delegates them to the available crew agents dynamically.
- **Flow:** Manager receives Task -> Delegates to Specialist -> Specialist returns Result -> Manager reviews/completes.
- **Use Case:** Complex problem solving where the specific steps or assignees are not known upfront, or require oversight.

```csharp
var crew = new CrewBuilder()
    .WithProcess(new HierarchicalProcess(managerAgent))
    // ...
    .Build();
```

In a hierarchical process, unassigned tasks are automatically routed to the Manager. The Manager uses the **Delegation Plugin** to assign work.

## Events & Observability

The Crew emits lifecycle events via `IEventSystem`:
- `CrewStarted` / `CrewCompleted`
- `TaskStarted` / `TaskCompleted`
- `AgentAction`

You can subscribe to these events for logging or real-time UI updates.

```csharp
var events = new EventSystem();
events.TaskCompleted += (sender, args) => Console.WriteLine($"Task finished: {args.Result}");
```
