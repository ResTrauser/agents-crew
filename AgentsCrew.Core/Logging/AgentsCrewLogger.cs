using System;
using Serilog;

namespace AgentsCrew.Core.Logging
{
    public class AgentsCrewLogger : IAgentsCrewLogger
    {
        private readonly ILogger _logger;

        public AgentsCrewLogger(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void LogInformation(string message, params object[] args)
        {
            _logger.Information(message, args);
        }

        public void LogWarning(string message, params object[] args)
        {
            _logger.Warning(message, args);
        }

        public void LogError(Exception ex, string message, params object[] args)
        {
            _logger.Error(ex, message, args);
        }

        public void LogDebug(string message, params object[] args)
        {
            _logger.Debug(message, args);
        }
    }
}
