# Documentación Completa de CrewAI.DotNet

## 1. Introducción y Objetivo del Proyecto
**CrewAI.DotNet** es un framework diseñado para orquestar agentes de inteligencia artificial autónomos que asumen roles específicos dentro de .NET. 

**¿Por qué nace este proyecto?** 
El ecosistema de Python se ha visto enriquecido enormemente por herramientas de orquestación multi-agentes como **CrewAI** original, que permiten a los desarrolladores crear sistemas donde múltiples agentes de IA colaboran para lograr un objetivo complejo. El ecosistema de .NET, especialmente en entornos corporativos orientados a objetos, necesitaba una solución nativa, fluida y poderosa que permitiera replicar este comportamiento de manera elegante. 

**Objetivo:**
Brindar una herramienta similar a CrewAI de Python, orientada a .NET, construida sobre **Microsoft Semantic Kernel**. Busca ser *fácil de usar* mediante una API fluida (Fluent API) basada en "Builders", y *muy poderosa* aprovechando las características avanzadas de C# y los plugins de Semantic Kernel.

---

## 2. Arquitectura General y Componentes Principales

El flujo de trabajo en CrewAI.DotNet sigue una estructura jerárquica y secuencial (o basada en procesos) que coordina los siguientes componentes principales:

### A. Agentes (Agents)
Los agentes son las entidades autónomas encargadas de realizar el trabajo. 
- **Rol (Role):** Define quién es el agente (ej. "Desarrollador Senior", "Investigador").
- **Objetivo (Goal):** Lo que el agente busca lograr a gran escala.
- **Historia / Contexto (Backstory):** Da personalidad y contexto profundo al LLM para responder de forma más alineada a su rol.
- **Delegación:** Los agentes (como un "Manager") pueden delegar tareas a otros agentes registrados usando un `AgentManager`.
- **Implementación:** Se construyen fácilmente usando el `AgentBuilder`. Requieren estar integrados con un `Kernel` de Semantic Kernel para su "cerebro" (el LLM).

### B. Tareas (Tasks)
Las tareas son las unidades de trabajo discretas que el Crew debe ejecutar.
- **Descripción:** Las instrucciones precisas del trabajo a realizar.
- **Salida Esperada (Expected Output):** Qué se espera que entregue la tarea cuando concluya.
- **Salida Estructurada:** Las tareas pueden definir un `OutputType` específico para que el modelo devuelva el resultado ya formateado/estructurado (por ejemplo, en un objeto C# nativo como `AppResult`).
- **Asignación:** Cada tarea se asigna a un Agente específico para su resolución utilizando `CrewTaskBuilder`.

### C. Equipo u Orquestador (Crew)
El "Crew" es el contenedor y ejecutor principal. 
- Agrupa a todos los Agentes y Tareas.
- Se puede configurar con un contexto de memoria (`MemoryContext`).
- Define el proceso de ejecución (por defecto secuencial, o jerárquico).
- Al llamar al método `KickoffAsync()`, el Crew inicia la orquestación, pasando los resultados de una tarea como contexto para la siguiente si es necesario, o permitiendo a los mánagers delegar.

---

## 3. Características Avanzadas y Módulos Funcionales

### 3.1. Sistema de Memoria (Memory)
Para que los agentes sean contextualmente inteligentes y recuerden interacciones previas, el framework integra un robusto sistema de memoria compuesto por tres capas (`MemoryContext`):
1. **Memoria a Corto Plazo (ShortTermMemory):** Almacena el contexto de la ejecución actual y el hilo de pensamiento reciente.
2. **Memoria a Largo Plazo (LongTermMemory):** Utiliza almacenamiento semántico y vectorial (como embeddings) para recordar información pasada más allá de la sesión funcional actual (RAG).
3. **Memoria de Entidades (EntityMemory):** Enfocada en extraer y recordar entidades específicas y datos estructurados del contexto (nombres, lugares, valores clave).

### 3.2. Herramientas y Plugins (Tools)
Gracias a su construcción sobre **Microsoft Semantic Kernel**, todas las herramientas en CrewAI.DotNet son básicamente Plugins del Kernel.
- Los agentes pueden usar funciones nativas de C# empaquetadas como Plugins, por ejemplo, realizar búsquedas web (`WebSearch`), leer o escribir archivos (`FileTool`), acceder a bases de datos, APIs de terceros, etc.
- Esto le otorga al LLM la capacidad de "actuar" sobre el mundo real (Acciones a través de C#).

### 3.3. Delegación Dinámica (Dynamic Delegation)
Mediante clases como `AgentManager` y la interfaz `IAgentManager`, los agentes con capacidades gerenciales pueden "subcontratar" o crear tareas al vuelo para enviarlas a agentes subordinados (por ejemplo, un Gerente de Proyecto delegando la creación de base de datos a un Desarrollador o el diseño a un Diseñador).

### 3.4. Integración de Conocimiento (Knowledge Sources)
Los agentes pueden ser alimentados con fuentes de conocimiento específicas (`IKnowledgeSource`, como un archivo de texto o base de datos externa) que actúan como "memoria especializada" pre-cargada. En el ejemplo, podemos ver el uso de `TextFileKnowledgeSource`.

---

## 4. Ejemplo Práctico (Flujo Funcional)
Para poner a funcionar un Crew, el desarrollador típico sigue este flujo (como se ve en `Program.cs`):

1. **Configuración del Kernel:** Inicializar Semantic Kernel, inyectar logging y el servicio de chat (OpenAI, Azure, local, etc.).
2. **Setup de Infraestructura:** Crear los manejadores de agentes y el contexto de memoria.
3. **Creación de Agentes:** Usando Fluent API (ej. `.WithRole("Manager").Build()`). Si un agente necesita leer archivos o web, se le inyectan los plugins de Semantic Kernel.
4. **Definición de Tareas:** Usando `CrewTaskBuilder`, se decribe la tarea y se enruta de inmediato a un agente.
5. **Ensamblado del Crew:** Usando `CrewBuilder`, se registran todas las partes creadas, dotándolo del contexto de memoria y proceso adecuado.
6. **Despegue (`KickoffAsync`):** El motor inicia la ejecución, las interacciones suceden y el resultado consolida la experiencia final.

---

## 5. Conclusión
**CrewAI.DotNet** democratiza la creación de aplicaciones inteligentes de Múltiples Agentes (Multi-Agent Systems) en la plataforma Microsoft. Utilizando código fuertemente tipado, patrones arquitectónicos conocidos en C# (Builders, Dependency Injection, Interfaces) y Microsoft Semantic Kernel como motor semántico base, logra ofrecer un símil exacto, y nativamente cómodo, del aclamado framework CrewAI original de Python.
