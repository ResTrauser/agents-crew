using System;
using Microsoft.Extensions.Logging;

namespace AgentsCrew.Core.Logging
{
    public class AgentsCrewLoggerProvider : ILoggerProvider
    {
        private readonly IAgentsCrewLogger _logger;

        public AgentsCrewLoggerProvider(IAgentsCrewLogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new AgentsCrewLoggerAdapter(_logger, categoryName);
        }

        public void Dispose()
        {
        }

        private class AgentsCrewLoggerAdapter : ILogger
        {
            private readonly IAgentsCrewLogger _logger;
            private readonly string _categoryName;

            public AgentsCrewLoggerAdapter(IAgentsCrewLogger logger, string categoryName)
            {
                _logger = logger;
                _categoryName = categoryName;
            }

            public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

            public bool IsEnabled(LogLevel logLevel) => true;

            public void Log<TState>(
                LogLevel logLevel,
                EventId eventId,
                TState state,
                Exception? exception,
                Func<TState, Exception?, string> formatter)
            {
                if (exception != null)
                {
                    _logger.LogError(exception, formatter(state, exception));
                }
                else
                {
                    switch (logLevel)
                    {
                        case LogLevel.Information:
                            _logger.LogInformation(formatter(state, exception));
                            break;
                        case LogLevel.Warning:
                            _logger.LogWarning(formatter(state, exception));
                            break;
                        case LogLevel.Error:
                            _logger.LogError(exception ?? new Exception(formatter(state, exception)), formatter(state, exception));
                            break;
                        case LogLevel.Debug:
                        case LogLevel.Trace:
                            _logger.LogDebug(formatter(state, exception));
                            break;
                    }
                }
            }
        }
    }
}
