# CrewAI.DotNet: Documentación Técnica de Arquitectura

## 1. Visión General e Introducción
**CrewAI.DotNet** es un framework de orquestación de agentes de Inteligencia Artificial diseñado específicamente para el ecosistema .NET. Su objetivo es proporcionar a los desarrolladores empresariales una forma estructurada, poderosa y nativa de construir sistemas multi-agente (Multi-Agent Systems - MAS).

Inspirado en el éxito de bibliotecas de Python como CrewAI, y cimentado sobre las bases sólidas de **Microsoft Semantic Kernel**, este framework permite crear flujos de trabajo donde múltiples IAs asumen "roles" específicos, comparten un contexto de memoria y colaboran en secuencias para resolver problemas complejos.

---

## 2. Arquitectura del Sistema

El diseño arquitectónico de CrewAI.DotNet se basa en patrones arraigados en C#, priorizando la flexibilidad, la inmutabilidad en la configuración y la extensibilidad. 

### 2.1. Cimiento Tecnológico
- **Microsoft Semantic Kernel (SK):** CrewAI.DotNet no implementa su propio cliente de IA. Delega el procesamiento de lenguaje natural y la ejecución de herramientas a `Kernel` de SK. Esto significa que cualquier modelo (OpenAI, Azure OpenAI, HuggingFace) o Plugin soportado por SK es inmediatamente compatible con CrewAI.DotNet.
- **Fluent Builder Pattern:** La construcción de los objetos complejos (`Agent`, `Crew`, `CrewTask`) se realiza mediante sus respectivos constructores fluidos (`AgentBuilder`, `CrewBuilder`, `CrewTaskBuilder`).
- **Programación Asíncrona:** El núcleo del pipeline de ejecución (`IProcess.ExecuteAsync`, `IAgent.ExecuteAsync`) utiliza implementaciones asíncronas no bloqueantes (`async/await`) para garantizar el rendimiento en entornos concurrentes.
- **Resiliencia:** Incorpora políticas de `Polly` para la tolerancia a fallos en las llamadas a los modelos de lenguaje.

### 2.2. Diagrama Conceptual de Dominio
1. Un **Crew** orquesta la ejecución.
2. Contiene una colección de **Agents** y una lista de **CrewTasks**.
3. Las tareas se procesan según la estrategia de un **IProcess** (ej. Secuencial).
4. Todo el flujo comparte y se nutre de un **MemoryContext** transversal y, opcionalmente, de fuentes de conocimiento estáticas (**KnowledgeSources**).

---

## 3. Componentes Principales y Clases Base

### 3.1. Agentes (`IAgent` y `Agent`)
El agente es la unidad cognitiva y ejecutora primaria.

**Propiedades Destacadas:**
*   `Role` (string): Identificador funcional (ej. "Arquitecto de Software").
*   `Goal` (string): Meta a largo plazo que orienta la toma de decisiones.
*   `Backstory` (string): Texto de trasfondo que define el _persona_ del prompt, logrando que el LLM actúe de manera hiper-específica.
*   `Kernel` (Kernel): Instancia de Semantic Kernel asignada al agente. Al inicializar, el agente crea un _scoped kernel_ para aislar los plugins exclusivos (`Tools`).
*   `AllowDelegation` (bool): Habilita o restringe la capacidad del agente para pasar trabajo a subordinados a través de `AgentManager`.

**Métodos Clave:**
*   `Task<string> ExecuteAsync(ICrewTask task, IMemoryContext context)`:
    Es el motor de inferencia. Construye el prompt inyectando el _Role_, _Goal_, _Backstory_, las instrucciones de salida estructurada, y el historial de `MemoryContext`. Invoca a Semantic Kernel configurado explícitamente con `ToolCallBehavior.AutoInvokeKernelFunctions` para que el modelo auto-resuelva qué herramientas usar.

### 3.2. Tareas (`ICrewTask` y `CrewTask`)
Una tarea es una encapsulación inmutable de trabajo a realizar.

**Propiedades Destacadas:**
*   `Description`: Prompt específico de lo que se debe hacer.
*   `ExpectedOutput`: Criterio de aceptación para elLLM.
*   `AssignedAgent`: Agente responsable de ejecutarla.
*   `OutputType` (Type): Permite forzar al modelo a devolver JSON puro con la estructura mapeada a una clase C# (Ej. `AppResult`). Se inyecta dinámicamente un esquema JSON en el prompt del agente.
*   `Context` (IList<ICrewTask>): Lista de tareas previas cuyos resultados (*Outputs*) serán concatenados y agregados al prompt de la tarea actual antes de ejecutarse.

### 3.3. Equipos / Orquestador (`ICrew` y `Crew`)
El coordinador de más alto nivel de la aplicación.

**Propiedades Destacadas:**
*   `Agents` y `Tasks`: Listados inmutables de los participantes y las metas.
*   `Process` (`IProcess`): Inyección de la estrategia de orquestación (ver 3.4).
*   `MemoryContext`: Inyección del estado de memoria compartida global.

**Métodos Clave:**
*   `Task KickoffAsync()`: Delega el inicio de la orquestación a un `CrewRunner` asilado.

### 3.4. Procesos y Ejecución (`IProcess`)
Define cómo la lista de tareas será consumida por los agentes.
*   **`SequentialProcess`**: (Implementado por defecto). Itera estrictamente sobre `IList<ICrewTask>`. Resuelve dinámicamente el `task.Context` inyectando los text outputs anteriores en el *Description* actual. Emite eventos a `OnTaskCompleted` en cada paso exitoso.
*   **`HierarchicalProcess`**: (Estructura proyectada). Permite que un `ManagerAgent` designado reciba las tareas y determine, en tiempo de ejecución, a qué agente delegarla.

### 3.5. Sistema de Memoria (`IMemoryContext`)
La inteligencia persistida que previene amnesia contextual del agente. Consolidado en `MemoryContext` agrupando:
*   **`IShortTermMemory`**: Almacenamiento rápido en RAM. Recolecta un log de los resultados concatenados durante el ciclo de vida de un `KickoffAsync()`.
*   **`ILongTermMemory`**: Abstracción sobre una base de datos vectorial (ej. `VolatileSemanticMemory` para pruebas, o implementaciones futuras con Qdrant/Pinecone). Empleado principalmente para RAG (Retrieval-Augmented Generation) integrando los `KnowledgeSources` estáticos que el Agente "ingesta" (`IngestKnowledgeAsync`) antes de actuar.
*   **`IEntityMemory`**: Extracción especializada de entidades relacionales del dominio del usuario.

### 3.6. Manejo de Agentes y Delegación (`IAgentManager`)
`AgentManager` es un registro global inyectado durante el `CrewBuilder`.
Sirve como directorio. Si un agente Manager detecta que no puede resolver una tarea, utiliza al `AgentManager` expuesto como Tool/Enrutador para comunicarse con otro `Agent` del listado registrado y pedir su soporte sin que el programador deba definir la interacción explícitamente.

---

## 4. Guía de Uso del Desarrollador (Developer Experience)

El diseño del API prioriza el patrón Fluent para maximizar la legibilidad del código.
```csharp
// 1. Inicializar Kernel (El motor LLM de Microsoft SK)
var kernel = Kernel.CreateBuilder().AddOpenAIChatCompletion(...).Build();

// 2. Definir Agentes con Builders funcionales
var researcher = new AgentBuilder()
    .WithRole("Investigador de IA")
    .WithGoal("Buscar la información más reciente sobre Frameworks Multi-Agentes")
    .WithBackstory("Eres un experto investigador en arquitecturas de IA empresariales.")
    .WithKernel(kernel)
    .Build();

// 3. Crear el plan de trabajo con dependencias (Contexto)
var taskResearch = new CrewTaskBuilder()
    .WithDescription("Investiga las tendencias actuales en C#.")
    .AssignTo(researcher)
    .Build();

// 4. Consolidar el Crew e Iniciar el proceso
var crew = new CrewBuilder()
    .AddAgent(researcher)
    .AddTask(taskResearch)
    .Build();

// Ejecución
await crew.KickoffAsync();
```

---

## 5. Extensibilidad

*   **Custom Tools**: Para agregar capacidades a un agente, basta con escribir un Plugin nativo de Semantic Kernel (clase regular con atributos `[KernelFunction]`) y agregarlo usando `.Tools.Add()`. CrewAI.DotNet expondrá esa función y la orquestará mediante *auto invoke*.
*   **Base de Datos Personalizadas**: Las implementaciones `IShortTermMemory` y `ILongTermMemory` pueden ser reemplazadas inyectando adaptadores de Redis o SQL Server al construir el `MemoryContext`.
