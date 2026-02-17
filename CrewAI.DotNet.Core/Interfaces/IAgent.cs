using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;

namespace CrewAI.DotNet.Core.Interfaces
{
    public interface IAgent
    {
        string Role { get; }
        string Goal { get; }
        string Backstory { get; }
        IList<KernelPlugin> Tools { get; }
        IList<IKnowledgeSource> KnowledgeSources { get; }

        Task<string> ExecuteAsync(ICrewTask task, IMemoryContext? memoryContext = null);
    }
}
