# Plan de Implementación: AgentsCrew

## Visión General

Convertir CrewAI.DotNet.Core en una framework enterprise-ready para .NET, con funcionalidades comparables a CrewAI Python y Microsoft Agent Framework.

| Aspecto | Cambio |
|---------|--------|
| **Framework** | .NET 8 → **.NET 10** |
| **Solution** | CrewAI.DotNet.sln → **AgentsCrew.sln** |
| **Paquete NuGet** | CrewAI.DotNet.Core → **AgentsCrew.Core** |
| **API Docs** | Swagger → **Scalar** |
| **Namespaces** | CrewAI.DotNet.* → **AgentsCrew.*** |

---

## Timeline Total: 20 semanas (5 meses)

| Fase | Semanas | Objetivo |
|------|---------|----------|
| Fase 1-2 | 1-8 | Fundamentos + Memoria Avanzada |
| Fase 3 | 9-12 | Workflows y Orquestación |
| Fase 4 | 13-16 | Integraciones Enterprise |
| Fase 5-6 | 17-20 | Observabilidad + Release |

---

# FASE 1-2: Fundamentos + Memoria Avanzada (Semanas 1-8)

## Semana 1: Renovación del Proyecto

### 1.1 Renombrar archivos y estructura

- CrewAI.DotNet.sln → AgentsCrew.sln
- CrewAI.DotNet.Core/ → AgentsCrew.Core/
- CrewAI.DotNet.Tools/ → AgentsCrew.Tools/
- CrewAI.DotNet.Tests/ → AgentsCrew.Tests/
- CrewAI.DotNet.Example/ → AgentsCrew.Example/

### 1.2 Actualizar .csproj principales

**AgentsCrew.Core.csproj:**
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <NoWarn>SKEXP0001</NoWarn>
    <PackageId>AgentsCrew.Core</PackageId>
    <Version>0.1.0-alpha</Version>
    <Authors>AgentsCrew Contributors</Authors>
    <Description>Multi-agent AI framework for .NET - Orchestrate autonomous AI agents with collaborative intelligence</Description>
    <RootNamespace>AgentsCrew.Core</RootNamespace>
    <RepositoryUrl>https://github.com/anomalyco/opencode</RepositoryUrl>
    <PackageLicenseExpression>MIT</PackageLicenseExpression>
  </PropertyGroup>
</Project>
```

**AgentsCrew.Tools.csproj:**
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <RootNamespace>AgentsCrew.Tools</RootNamespace>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="..\AgentsCrew.Core\AgentsCrew.Core.csproj" />
  </ItemGroup>
</Project>
```

**AgentsCrew.Tests.csproj:**
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
    <RootNamespace>AgentsCrew.Tests</RootNamespace>
  </PropertyGroup>
</Project>
```

**AgentsCrew.Example.csproj:**
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <OutputType>Exe</OutputType>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <RootNamespace>AgentsCrew.Example</RootNamespace>
  </PropertyGroup>
</Project>
```

### 1.3 Actualizar namespaces (Find & Replace global)

En todos los archivos .cs:
- `CrewAI.DotNet.Core` → `AgentsCrew.Core`
- `CrewAI.DotNet.Tools` → `AgentsCrew.Tools`
- `CrewAI.DotNet.Tests` → `AgentsCrew.Tests`
- `CrewAI.DotNet.Example` → `AgentsCrew.Example`
- `using CrewAI.DotNet` → `using AgentsCrew`

### 1.4 Actualizar AGENTS.md

Cambiar referencias:
- Commands: `dotnet build AgentsCrew.sln`
- `dotnet test AgentsCrew.sln`
- Project structure: AgentsCrew.Core, AgentsCrew.Tools, etc.

---

## Semana 2: Logging + Scalar + Error Handling

### 2.1 Paquetes NuGet a agregar

**AgentsCrew.Core.csproj:**
```xml
<PackageReference Include="Serilog.AspNetCore" Version="8.0.0" />
<PackageReference Include="Serilog.Sinks.Console" Version="5.0.1" />
<PackageReference Include="Serilog.Sinks.File" Version="5.0.0" />
<PackageReference Include="Polly" Version="8.6.0" />
<PackageReference Include="Polly.Extensions" Version="8.6.0" />
```

**AgentsCrew.Example.csproj:**
```xml
<PackageReference Include="Scalar.AspNetCore" Version="1.0.0" />
```

### 2.2 Estructura de Logging

```
AgentsCrew.Core/Logging/
├── IAgentsCrewLogger.cs
├── AgentsCrewLogger.cs
├── AgentsCrewLoggerProvider.cs
└── LoggingExtensions.cs
```

**IAgentsCrewLogger.cs:**
```csharp
using System;

namespace AgentsCrew.Core.Logging
{
    public interface IAgentsCrewLogger
    {
        void LogInformation(string message, params object[] args);
        void LogWarning(string message, params object[] args);
        void LogError(Exception ex, string message, params object[] args);
        void LogDebug(string message, params object[] args);
    }
}
```

### 2.3 Actualizar ResiliencePolicies

**AgentsCrew.Core/Configuration/ResiliencePolicies.cs:**
- Retry Policy: exponential backoff, 3 retries, 1s base delay
- Circuit Breaker: 5 failures → 30s break
- Timeout Policy: 120s para LLM
- Fallback Policy: default response
- Composite Policy: todo combinado

### 2.4 Configurar Scalar en Example

**AgentsCrew.Example/Program.cs:**
```csharp
app.UseScalar(new ScalarOptions
{
    DefaultOpenApiVersion = ScalarOpenApiVersion.V3
});
app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));
```

Endpoints:
- `/api/agents` - gestión de agentes
- `/api/crew` - gestión de crews
- `/scalar/v1` - documentación API interactiva
- `/health` - health checks

---

## Semana 3-4: Sistema de Memoria V2 (Vector Stores)

### 3.1 Estructura VectorStores

```
AgentsCrew.Core/Memory/VectorStores/
├── Interfaces/
│   ├── IVectorStore.cs
│   ├── IVectorRecord.cs
│   └── IEmbeddingGenerator.cs
├── Implementations/
│   ├── QdrantVectorStore.cs
│   ├── InMemoryVectorStore.cs
│   ├── AzureAISearchVectorStore.cs
│   └── PineconeVectorStore.cs
└── DependencyInjection/
    └── VectorStoreServiceCollectionExtensions.cs
```

### 3.2 IVectorStore Interface

```csharp
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AgentsCrew.Core.Memory.VectorStores
{
    public interface IVectorStore
    {
        Task<string> UpsertAsync(string collection, IVectorRecord record);
        Task<IReadOnlyList<IVectorRecord>> SearchAsync(
            string collection,
            ReadOnlyMemory<float> embedding,
            int topK = 4);
        Task DeleteAsync(string collection, string id);
        Task<bool> CollectionExistsAsync(string collection);
        Task CreateCollectionAsync(string collection, int dimensions);
    }

    public interface IVectorRecord
    {
        string Id { get; set; }
        ReadOnlyMemory<float> Embedding { get; set; }
        string? Content { get; set; }
        IDictionary<string, object>? Metadata { get; set; }
    }
}
```

### 3.3 Implementaciones

- **QdrantVectorStore**: Migrar de LongTermMemory existente, usar Qdrant.Client v1.17.0
- **InMemoryVectorStore**: Diccionario simple para testing/dev sin Docker
- **AzureAISearchVectorStore**: Integración Azure.Search.Documents
- **PineconeVectorStore**: Integración con Pinecone SDK

### 3.4 Selector de Vector Store

```csharp
// En MemoryContext builder
builder.WithVectorStore(new QdrantVectorStore(config));    // Default
builder.WithVectorStore(new InMemoryVectorStore());        // Testing
builder.WithVectorStore(new AzureAISearchVectorStore(c));  // Enterprise
builder.WithVectorStore(new PineconeVectorStore(c));       // Cloud managed
```

---

## Semana 5-6: Embeddings + Knowledge Sources

### 4.1 Proveedores de Embeddings

```
AgentsCrew.Core/Embeddings/
├── Interfaces/
│   ├── IEmbeddingGenerator.cs
│   └── EmbeddingResult.cs
├── Providers/
│   ├── OpenAIEmbeddingGenerator.cs
│   ├── AzureOpenAIEmbeddingGenerator.cs
│   └── HuggingFaceEmbeddingGenerator.cs
└── DependencyInjection/
    └── EmbeddingGeneratorExtensions.cs
```

### 4.2 Knowledge Sources Multi-Formato

```
AgentsCrew.Core/Knowledge/
├── IKnowledgeSource.cs (actualizar namespace)
├── BaseKnowledgeSource.cs (clase base NUEVA)
├── TextFileKnowledgeSource.cs (actualizar)
├── JsonKnowledgeSource.cs (NUEVO)
├── PdfKnowledgeSource.cs (NUEVO - con PDFSharp)
├── XmlKnowledgeSource.cs (NUEVO)
├── WebKnowledgeSource.cs (NUEVO - HttpClient)
├── DatabaseKnowledgeSource.cs (NUEVO - SQL)
└── ChunkingStrategies/
    ├── FixedSizeChunker.cs
    ├── SentenceChunker.cs
    └── SemanticChunker.cs (NUEVO)
```

### 4.3 MemoryContext V2

**AgentsCrew.Core/Memory/MemoryContext.cs:**
```csharp
namespace AgentsCrew.Core.Memory
{
    public class MemoryContext : IMemoryContext
    {
        public IShortTermMemory ShortTerm { get; }
        public ILongTermMemory LongTerm { get; }
        public IEntityMemory Entity { get; }
        public ISessionMemory Session { get; }  // NUEVO

        // Métodos de alto nivel
        public async Task RememberAsync(string key, string value);
        public async Task<string> RecallAsync(string query);
        public async Task<EntityExtraction> ExtractEntitiesAsync(string text);
        public async Task<string> SummarizeAsync();
    }
}
```

### 4.4 Entity Memory Enhancement

Extraer entidades del texto:
- Personas
- Fechas
- Lugares
- Organizaciones
- Relaciones

### 4.5 Session Management

```
AgentsCrew.Core/Memory/
├── ISessionMemory.cs
├── InMemorySession.cs (default)
└── CosmosSession.cs (enterprise)
```

---

## Semana 7-8: Tests + Demo + Publicación Alpha

### 5.1 Tests de Integración

```
AgentsCrew.Tests/
├── VectorStoreTests.cs
│   ├── QdrantVectorStore_InsertAndSearch
│   ├── InMemoryVectorStore_InsertAndSearch
│   └── VectorStore_CollectionManagement
├── EmbeddingGeneratorTests.cs
│   ├── OpenAI_GenerateEmbedding
│   └── MockEmbeddingGenerator
├── KnowledgeSourceTests.cs
│   ├── JsonKnowledgeSource_Load
│   ├── ChunkingStrategies_Test
│   └── TextFileKnowledgeSource_Load
├── ResilienceTests.cs
│   ├── RetryPolicy_RetriesOnFailure
│   ├── CircuitBreaker_OpensAfterFailures
│   └── Timeout_CancelsLongRunning
└── EndToEndTests.cs (actualizar existentes)
```

### 5.2 Demo/Example Actualizado

**AgentsCrew.Example/Program.cs:**
- Configurar Serilog como logger
- Configurar Scalar para API docs
- Health endpoint
- Demo completo de memoria con vector store

**AgentsCrew.Example/appsettings.json:**
```json
{
  "VectorStore": {
    "Provider": "Qdrant",
    "Endpoint": "http://localhost:6333"
  },
  "Embedding": {
    "Provider": "OpenAI",
    "Model": "text-embedding-ada-002"
  },
  "LLM": {
    "Provider": "OpenAI",
    "Model": "gpt-4o"
  }
}
```

**AgentsCrew.Example/Docker-compose.yml:**
```yaml
services:
  qdrant:
    image: qdrant/qdrant:latest
    ports:
      - "6333:6333"
      - "6334:6334"
    volumes:
      - qdrant_data:/qdrant/storage

volumes:
  qdrant_data:
```

### 5.3 Publicación Alpha

Steps:
1. `dotnet build AgentsCrew.sln --configuration Release`
2. `dotnet pack AgentsCrew.Core --configuration Release`
3. `dotnet nuget push *.nupkg --source https://api.nuget.org/v3/index.json`
4. Crear GitHub Release v0.2.0-alpha con release notes

---

# FASE 3: Workflows y Orquestación (Semanas 9-12)

## Objetivo: Graph-based workflows + checkpoints

### 3.1 Graph Workflow Engine (Semanas 9-10)

```
AgentsCrew.Core/Workflows/
├── Interfaces/
│   ├── IWorkflow.cs
│   ├── IWorkflowNode.cs
│   ├── IWorkflowEdge.cs
│   └── IWorkflowState.cs
├── Engine/
│   ├── WorkflowEngine.cs
│   ├── WorkflowGraph.cs
│   └── WorkflowExecutor.cs
├── Nodes/
│   ├── AgentNode.cs
│   ├── ConditionNode.cs
│   ├── HumanApprovalNode.cs
│   └── FunctionNode.cs
├── State/
│   ├── WorkflowState.cs
│   └── StateManager.cs
└── Builders/
    └── WorkflowBuilder.cs (fluent API)
```

### 3.2 Type-Safe Routing (Semana 10)

```csharp
var workflow = new WorkflowBuilder()
    .AddNode("research", new AgentNode(researcher))
    .AddNode("write", new AgentNode(writer))
    .AddEdge("research", "write", (result) => result.Length > 100)
    .Build();
```

### 3.3 Checkpoint/Resume (Semana 11)

```
AgentsCrew.Core/Workflows/Checkpoint/
├── ICheckpointStore.cs
├── InMemoryCheckpointStore.cs
├── FileSystemCheckpointStore.cs
└── CosmosDbCheckpointStore.cs
```

### 3.4 Human-in-the-Loop (Semana 11-12)

```
AgentsCrew.Core/Workflows/HumanApproval/
├── IHumanApprovalProvider.cs
├── ConsoleApprovalProvider.cs
├── HttpApprovalProvider.cs
└── ApprovalRequest.cs
```

---

# FASE 4: Integraciones Enterprise (Semanas 13-16)

## Objetivo: Conexión con ecosistema Azure

### 4.1 Azure AI Foundry (Semanas 13-14)

```
AgentsCrew.Core/Integrations/Azure/
├── AzureAIAgentProvider.cs
├── AzureFoundryClient.cs
└── AzureExtensions.cs
```

### 4.2 MCP Client (Semanas 14-15)

```
AgentsCrew.Core/Integrations/MCP/
├── IMCPClient.cs
├── MCPClient.cs
├── MCPToolAdapter.cs
└── MCPExtensions.cs
```

### 4.3 Autenticación Azure AD (Semana 15)

```
AgentsCrew.Core/Security/
├── IAuthProvider.cs
├── AzureAdAuthProvider.cs
└── SecurityExtensions.cs
```

### 4.4 Event Grid/Service Bus (Semana 16)

```
AgentsCrew.Core/Events/
├── IEventPublisher.cs
├── EventGridPublisher.cs
├── ServiceBusPublisher.cs
└── EventExtensions.cs
```

---

# FASE 5-6: Observabilidad + Release (Semanas 17-20)

## 5.1 Health Checks (Semana 17)

```
AgentsCrew.Core/Health/
├── AgentHealthCheck.cs
├── VectorStoreHealthCheck.cs
├── LLMHealthCheck.cs
└── HealthExtensions.cs
```

## 5.2 Metrics (Semana 17-18)

```
AgentsCrew.Core/Metrics/
├── AgentsCrewMetrics.cs
├── PrometheusExporter.cs
└── GrafanaDashboard.json
```

## 5.3 Distributed Tracing (Semana 18)

- OpenTelemetry con Zipkin/Jaeger
- Trace IDs en logs
- Span por operación de agente

## 5.4 Alerting (Semana 19)

- Configuración de alertas
- Webhook para notificaciones

## 5.5 Audit Logging (Semana 19)

```
AgentsCrew.Core/Audit/
├── IAuditLogger.cs
├── AuditLogger.cs
└── AuditRecord.cs
```

## 5.6 Release v1.0.0 (Semana 20)

- Documentación API completa (Scalar)
- Samples de producción
- CI/CD con GitHub Actions
- NuGet publish estable

---

## ✅ Entregables Finales

| Fase | Entregable | Estado |
|------|------------|--------|
| 1-2 | Solution .NET 10 | ⬜ |
| 1-2 | NuGet v0.2.0-alpha | ⬜ |
| 1-2 | Logging estructurado (Serilog) | ⬜ |
| 1-2 | Error handling (Polly) | ⬜ |
| 1-2 | 4 Vector Stores | ⬜ |
| 1-2 | 3 Embedding providers | ⬜ |
| 1-2 | 6 Knowledge sources | ⬜ |
| 1-2 | Scalar API docs | ⬜ |
| 3 | Graph Workflows | ⬜ |
| 3 | Checkpoint/Resume | ⬜ |
| 3 | Human-in-the-Loop | ⬜ |
| 4 | Azure AI Foundry | ⬜ |
| 4 | MCP Client | ⬜ |
| 4 | Azure AD Auth | ⬜ |
| 5-6 | Health + Metrics | ⬜ |
| 5-6 | Distributed Tracing | ⬜ |
| 5-6 | NuGet v1.0.0-stable | ⬜ |

---

## 📊 Comparativa Objetivo

| Feature | CrewAI Python | Microsoft Agent Framework | AgentsCrew .NET |
|---------|---------------|---------------------------|-----------------|
| Agentes + Tasks | ✅ | ✅ | ✅ (actual) |
| Memory (short/long/entity) | ✅ | ✅ | ✅ (actual) |
| Knowledge Sources | ✅ | ❌ | ✅ (Fase 2) |
| Vector Stores | ✅ | ✅ | ✅ (Fase 2) |
| Graph Workflows | ❌ | ✅ | ✅ (Fase 3) |
| Human-in-the-Loop | ❌ | ✅ | ✅ (Fase 3) |
| Azure Integration | ❌ | ✅ | ✅ (Fase 4) |
| MCP Protocol | ❌ | ✅ | ✅ (Fase 4) |
| Production Ready | ✅ | RC | v1.0 (Fase 6) |

---

## Notas Importantes

1. **.NET 10** - Verificar disponibilidad (lanzamiento previsto ~Nov 2025)
2. **Qdrant** - Docker local para dev, cloud para prod
3. **Scalar** - Requiere ASP.NET Core (solo en Example, no en Core)
4. **PDFSharp** - Para parsing PDF
5. **Trabajo solo** - Timeline realista para 1 persona
