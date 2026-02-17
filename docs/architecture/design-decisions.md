# Design Decisions

## 1. Why Semantic Kernel?

We chose **Microsoft Semantic Kernel** (SK) as the underlying AI orchestration engine because:
- **Native .NET Support:** It's the official Microsoft library for integrating LLMs into .NET apps.
- **Plugin System:** Simplifies tool creation and management.
- **Memory Abstractions:** Provides `ISemanticTextMemory` for vector search out-of-the-box.
- **Connector Ecosystem:** Supports OpenAI, Azure OpenAI, HuggingFace, and more.

## 2. Event System

Instead of tightly coupling logging or UI updates, we implemented an `IEventSystem`. This allows:
- Decoupling execution logic from side effects.
- Flexibility for different consumers (Console, Web API, GUI).
- Standard .NET event pattern (`EventHandler<T>`).

## 3. Delegation as a Tool

Delegation is implemented as a **Plugin** (`DelegationPlugin`) rather than hardcoded logic. This empowers the LLM to *decide* when to delegate based on the task description and context, making the system more autonomous and flexible. The Manager agent simply has this tool available in its toolbox.

## 4. Scoped Kernels

To ensure thread safety and prevent state pollution (e.g., chat history or temporary plugins) between agents:
- Each `Agent.ExecuteAsync` call creates a **Scoped Kernel** (via `Kernel.Clone()`).
- Plugins specific to that execution (like ephemeral tools) are added to the scoped instance.
- Base services (LLM connector, Memory store) are shared but thread-safe.

## 5. YAML Configuration

We support YAML to align with the Python CrewAI ecosystem and enable configuration-driven development, where non-technical users can define agent personas and tasks.
