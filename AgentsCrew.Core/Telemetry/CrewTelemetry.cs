using System.Diagnostics;

namespace AgentsCrew.Core.Telemetry
{
    public static class CrewTelemetry
    {
        public const string ActivitySourceName = "AgentsCrew";
        public static readonly ActivitySource Source = new ActivitySource(ActivitySourceName, "1.0.0");
    }
}
