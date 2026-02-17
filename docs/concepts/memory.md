# Memory System

CrewAI .NET implements a sophisticated memory architecture to give agents context and continuity. The memory is managed via `IMemoryContext` and is shared across the Crew execution.

## 1. Short-Term Memory

- **Purpose:** Context window for the current execution run.
- **Storage:** In-memory list (Volatile).
- **Usage:** Stores the input task description and the results of all completed tasks in the current session.
- **Mechanism:** When an agent starts a task, the entire Short-Term Memory log is injected into the prompt as "Context".

## 2. Long-Term Memory (RAG)

- **Purpose:** Persistent knowledge base and semantic search.
- **Storage:** Vector Store (Embeddings).
- **Usage:**
    - Stores task results for future reference (across different runs).
    - Stores ingested **Knowledge Sources** (documents, text files).
- **Mechanism:**
    - Uses Semantic Kernel's `ISemanticTextMemory`.
    - Before task execution, the agent performs a semantic search using the Task Description as the query.
    - Relevant snippets (chunks) are retrieved and injected into the "Context" section of the prompt.
- **Integration:** Can be backed by any vector DB supported by Semantic Kernel (Azure AI Search, Qdrant, Pinecone, Volatile/In-Memory).

## 3. Entity Memory

- **Purpose:** Tracking specific entities (People, Places, Concepts) mentioned during the conversation.
- **Storage:** Key-Value store.
- **Usage:** ensures consistency in how entities are referred to or details about them.
- **Mechanism:** Currently a basic dictionary implementation. Future enhancements will include automatic entity extraction and resolution.

## Configuration

Memory is configured when building the Crew.

```csharp
var memory = new MemoryContext(
    new ShortTermMemory(),
    new LongTermMemory(mySemanticTextMemory),
    new EntityMemory()
);

var crew = new CrewBuilder()
    .WithMemory(memory)
    .Build();
```
