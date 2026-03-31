using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AgentsCrew.Core.Logging
{
    public static class LoggingExtensions
    {
        public static ILoggingBuilder AddAgentsCrew(this ILoggingBuilder builder)
        {
            builder.Services.AddSingleton<IAgentsCrewLogger, AgentsCrewLogger>();
            builder.Services.AddSingleton<ILoggerProvider, AgentsCrewLoggerProvider>();
            return builder;
        }
    }
}
