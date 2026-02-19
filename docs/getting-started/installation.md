# Installation & Prerequisites

## Prerequisites

- **.NET SDK 8.0** or higher.
- A valid **OpenAI API Key** (or Azure OpenAI endpoint) if using real LLM models.
- Basic understanding of C# and Dependency Injection.

## Installation

This framework is currently distributed as a source-available library. You can clone the repository and reference the `AgentsCrew.Core` project.

1.  **Clone the Repository:**
    ```bash
    git clone https://github.com/your-org/crewai-dotnet.git
    cd crewai-dotnet
    ```

2.  **Add Reference to Your Project:**
    If you are creating a new console application:
    ```bash
    dotnet new console -n MyCrewApp
    dotnet add MyCrewApp/MyCrewApp.csproj reference AgentsCrew.Core/AgentsCrew.Core.csproj
    ```

3.  **Install Dependencies:**
    Ensure you have the required Semantic Kernel packages:
    ```bash
    dotnet add package Microsoft.SemanticKernel --version 1.18.0
    dotnet add package YamlDotNet --version 16.3.0
    ```

## Next Steps

Check out the [Quickstart Guide](quickstart.md) to build your first Crew!
