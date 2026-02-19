using System.Collections.Generic;

namespace AgentsCrew.Core.Configuration
{
    public class CrewConfig
    {
        public List<AgentConfig> Agents { get; set; } = new List<AgentConfig>();
        public List<TaskConfig> Tasks { get; set; } = new List<TaskConfig>();
    }
}
