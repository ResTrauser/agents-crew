# AGENTS.md - AgentsCrew (.NET 10.0)

## Build & Test Commands

```bash
# Build entire solution
dotnet build AgentsCrew.sln

# Run all tests
dotnet test AgentsCrew.sln

# Run single test by name
dotnet test --filter "FullyQualifiedName~EndToEndTests.CrewExecution_WithMockLLM_ShouldCompleteTasks"

# Run tests in specific class
dotnet test --filter "FullyQualifiedName~AgentsCrew.Tests.EndToEndTests"

# Run tests with verbose output
dotnet test --verbosity normal

# Build specific project
dotnet build AgentsCrew.Core/AgentsCrew.Core.csproj

# Restore packages
dotnet restore
```

## Project Structure

- **AgentsCrew.Core** - Core library (models, builders, interfaces, memory, telemetry)
- **AgentsCrew.Tools** - Tool plugins (Docker, file I/O, web search)
- **AgentsCrew.Tests** - xUnit test project
- **AgentsCrew.Example** - Console example app

## Code Style Guidelines

### Formatting & Conventions
- Target: .NET 10.0 with nullable reference types enabled (`<Nullable>enable</Nullable>`)
- Implicit usings enabled (`<ImplicitUsings>enable</ImplicitUsings>`)
- Use block-scoped namespaces: `namespace X { }` (not file-scoped)
- Indentation: 4 spaces
- Brace style: Allman (new line for opening brace)
- Max line length: ~120 chars preferred

### Naming Conventions
- **Classes/Interfaces**: PascalCase (`Agent`, `IAgent`, `CrewTaskBuilder`)
- **Public properties**: PascalCase (`Role`, `Goal`, `Backstory`)
- **Private fields**: `_camelCase` prefix (`_role`, `_goal`, `_kernel`)
- **Method parameters**: camelCase (`task`, `memoryContext`, `cancellationToken`)
- **Local variables**: camelCase (`context`, `output`, `stopWatch`)
- **Constants**: PascalCase (`ActivitySourceName`)
- **Async methods**: Suffixed with `Async` (`ExecuteAsync`, `KickoffAsync`)

### Imports & Dependencies
- System imports first, then third-party, then project namespaces
- Group using statements at top of file
- Use fully-qualified names for `System.Action`, `System.Threading.CancellationToken` when needed to avoid conflicts

### Types & Nullability
- Enable nullable reference types throughout
- Use `?` suffix for nullable types (`Kernel?`, `IMemoryContext?`, `ILogger<IAgent>?`)
- Prefer `IList<T>` or `IReadOnlyList<T>` for collections exposed via interfaces
- Use `List<T>` internally and for builder accumulation
- Use `string.Empty` instead of `""` for empty strings
- Check for null with `== null` pattern, use `is null` / `is not null` for pattern matching

### Async Patterns
- All I/O and long-running operations return `Task<T>` or `IAsyncEnumerable<T>`
- Use `CancellationToken` parameter named `cancellationToken` with default value
- Use `[EnumeratorCancellation]` attribute on `IAsyncEnumerable` parameters
- Prefer `await foreach` for async streams
- Use `Task.FromResult<T>()` for sync-to-async wrapping in mocks

### Error Handling
- Throw `InvalidOperationException` for invalid state (e.g., null Kernel)
- Use try-catch around file I/O with logging or console output
- Log errors before re-throwing (observe-then-throw pattern)
- Use Polly resilience policies for transient failures
- Add OpenTelemetry activity tags for error status: `activity?.SetStatus(ActivityStatusCode.Error, ex.Message)`

### Builder Pattern
- Fluent builder APIs return `this` for method chaining
- Builder methods prefixed with `With` or `Add` (`WithRole`, `AddAgent`, `AddTool`)
- `Build()` method returns the constructed interface type (`IAgent`, `ICrew`)
- Store intermediate state in private fields, populate object in `Build()`

### Testing (xUnit)
- Use `[Fact]` for single test cases, `[Theory]` for parameterized
- Follow Arrange-Act-Assert pattern with comments
- Use `Assert.NotNull`, `Assert.Equal`, `Assert.Contains` for assertions
- Create private mock classes within test class scope
- Prefix test class with feature being tested: `EndToEndTests`, `SemanticKernelInteropTests`
- Test method names: `MethodOrFeature_WithCondition_ExpectedResult`

### Interfaces & Dependency Injection
- Define contracts in `Interfaces/` folder with `I` prefix
- Use `ILogger<T>` for logging injection
- Accept dependencies as constructor parameters or properties
- Prefer interface types for public API surface

### Telemetry & Observability
- Use `AgentsCrewTelemetry.Source.StartActivity()` for distributed tracing
- Set tags with `activity?.SetTag("key", value)`
- Use `Stopwatch` for timing execution
- Store metrics in dedicated `UsageMetrics` class

### Project References
- Core project is the base dependency for all other projects
- Tools project references Core only
- Test project references both Core and Tools
- Example project references Core and Tools, includes YAML config

### Special Considerations
- Suppress SKEXP0001/SKEXP0010 warnings for experimental SK features with `#pragma warning disable`
- Kernel.Clone() for scoped kernel instances to avoid modifying shared state
- Use `KernelPluginFactory.CreateFromObject()` to wrap C# objects as plugins
- YAML configuration loaded via `YamlConfigurationLoader` for external config
- Use Scalar for API documentation instead of Swagger
- Qdrant is the default vector store, with interfaces for flexibility to others
