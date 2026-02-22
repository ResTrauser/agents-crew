using Polly;
using System;
using System.Threading.Tasks;

namespace CrewAI.DotNet.Core.Configuration
{
    public static class ResiliencePolicies
    {
        public static IAsyncPolicy<T> GetDefaultRetryPolicy<T>(int maxRetries = 3)
        {
            return Policy<T>
                .Handle<Exception>() // Customize this handle for HttpRequestException, TimeoutException
                .WaitAndRetryAsync(
                    retryCount: maxRetries,
                    sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                    onRetry: (exception, timeSpan, retryCount, context) =>
                    {
                        // En un escenario de producción esto debe publicarse a un ILogger
                        Console.WriteLine($"[Polly] Retry {retryCount} due to {exception.Exception.Message}. Waiting {timeSpan}");
                    });
        }
    }
}
