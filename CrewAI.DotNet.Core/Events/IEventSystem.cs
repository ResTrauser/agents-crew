using CrewAI.DotNet.Core.Interfaces;

namespace CrewAI.DotNet.Core.Events
{
    public class CrewEventArgs : EventArgs
    {
        public string CrewName { get; }

        public CrewEventArgs(string crewName)
        {
            CrewName = crewName;
        }
    }

    public class TaskEventArgs : EventArgs
    {
        public ICrewTask Task { get; }
        public IAgent Agent { get; }
        public string? Result { get; }

        public TaskEventArgs(ICrewTask task, IAgent agent, string? result = null)
        {
            Task = task;
            Agent = agent;
            Result = result;
        }
    }

    public class AgentActionEventArgs : EventArgs
    {
        public IAgent Agent { get; }
        public string Action { get; }
        public string Input { get; }

        public AgentActionEventArgs(IAgent agent, string action, string input)
        {
            Agent = agent;
            Action = action;
            Input = input;
        }
    }

    public interface IEventSystem
    {
        event EventHandler<CrewEventArgs> CrewStarted;
        event EventHandler<CrewEventArgs> CrewCompleted;
        event EventHandler<TaskEventArgs> TaskStarted;
        event EventHandler<TaskEventArgs> TaskCompleted;
        event EventHandler<AgentActionEventArgs> AgentAction;

        void OnCrewStarted(string crewName);
        void OnCrewCompleted(string crewName);
        void OnTaskStarted(ICrewTask task, IAgent agent);
        void OnTaskCompleted(ICrewTask task, IAgent agent, string result);
        void OnAgentAction(IAgent agent, string action, string input);
    }
}
