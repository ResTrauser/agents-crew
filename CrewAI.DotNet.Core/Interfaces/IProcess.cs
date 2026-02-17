namespace CrewAI.DotNet.Core.Interfaces
{
    public interface IProcess
    {
        Task ExecuteAsync(
            IList<IAgent> agents,
            IList<ICrewTask> tasks,
            IMemoryContext? memoryContext = null
        );
    }
}
