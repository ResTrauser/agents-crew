using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrewAI.DotNet.Core.Interfaces
{
    public interface ICrew
    {
        IList<IAgent> Agents { get; }
        IList<ICrewTask> Tasks { get; }
        IProcess Process { get; }
        IMemoryContext MemoryContext { get; }

        Task KickoffAsync();
    }
}
