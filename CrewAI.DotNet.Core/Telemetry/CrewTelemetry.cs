using System.Diagnostics;

namespace CrewAI.DotNet.Core.Telemetry
{
    public static class CrewTelemetry
    {
        public const string ActivitySourceName = "CrewAI.DotNet";
        public static readonly ActivitySource Source = new ActivitySource(ActivitySourceName, "1.0.0");
    }
}
