using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using CrewAI.DotNet.Core.Telemetry;
using Microsoft.Extensions.Logging;

namespace CrewAI.DotNet.Core.Interfaces
{
    public interface IAgent
    {
        string Role { get; }
        string Goal { get; }
        string Backstory { get; }
        int MaxIter { get; set; }
        System.Action<string>? StepCallback { get; set; }
        bool AllowDelegation { get; set; }
        bool Cache { get; set; }
        IList<KernelPlugin> Tools { get; }
        IList<IKnowledgeSource> KnowledgeSources { get; }

        // Paridad CrewAI
        System.TimeSpan? MaxExecutionTime { get; set; }
        UsageMetrics Metrics { get; }
        ILogger<IAgent>? Logger { get; set; }

        Task<string> ExecuteAsync(ICrewTask task, IMemoryContext? memoryContext = null, System.Threading.CancellationToken cancellationToken = default);
        IAsyncEnumerable<string> ExecuteStreamingAsync(ICrewTask task, IMemoryContext? memoryContext = null, System.Threading.CancellationToken cancellationToken = default);
    }
}
