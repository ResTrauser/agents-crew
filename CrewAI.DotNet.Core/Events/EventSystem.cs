using System;
using CrewAI.DotNet.Core.Interfaces;

namespace CrewAI.DotNet.Core.Events
{
    public class EventSystem : IEventSystem
    {
        public event EventHandler<CrewEventArgs>? CrewStarted;
        public event EventHandler<CrewEventArgs>? CrewCompleted;
        public event EventHandler<TaskEventArgs>? TaskStarted;
        public event EventHandler<TaskEventArgs>? TaskCompleted;
        public event EventHandler<AgentActionEventArgs>? AgentAction;

        public void OnCrewStarted(string crewName) => CrewStarted?.Invoke(this, new CrewEventArgs(crewName));
        public void OnCrewCompleted(string crewName) => CrewCompleted?.Invoke(this, new CrewEventArgs(crewName));
        public void OnTaskStarted(ICrewTask task, IAgent agent) => TaskStarted?.Invoke(this, new TaskEventArgs(task, agent));
        public void OnTaskCompleted(ICrewTask task, IAgent agent, string result) => TaskCompleted?.Invoke(this, new TaskEventArgs(task, agent, result));
        public void OnAgentAction(IAgent agent, string action, string input) => AgentAction?.Invoke(this, new AgentActionEventArgs(agent, action, input));
    }
}
