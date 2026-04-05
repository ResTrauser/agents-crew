using System.Collections.Generic;
using System.Threading.Tasks;

namespace AgentsCrew.Core.Interfaces
{
    public interface IProcess
    {
        Task ExecuteAsync(IList<IAgent> agents, IList<ICrewTask> tasks, IMemoryContext? memoryContext = null);
    }
}
