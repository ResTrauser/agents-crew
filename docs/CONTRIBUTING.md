# Contributing to CrewAI .NET

We welcome contributions! Please follow these guidelines.

## Development Environment

- **.NET 8.0 SDK**
- An IDE (Visual Studio 2022, VS Code, or Rider).
- Git.

## Building the Project

1.  Clone the repo.
2.  Run `dotnet restore`.
3.  Run `dotnet build`.

## Running Tests

We use **xUnit**. Run tests via CLI:

```bash
dotnet test
```

Ensure all tests pass before submitting a PR.

## Code Style

- Follow standard C# naming conventions (PascalCase for classes/methods, camelCase for variables).
- Use `async/await` for I/O operations.
- Use nullable reference types (`<Nullable>enable</Nullable>`).

## Submitting a Pull Request

1.  Fork the repository.
2.  Create a feature branch (`git checkout -b feature/amazing-feature`).
3.  Commit your changes.
4.  Push to the branch.
5.  Open a Pull Request.
6.  Describe your changes and link to any relevant issues.

## Reporting Issues

Please use the GitHub Issues tracker to report bugs or request features. Provide a clear description and steps to reproduce.
