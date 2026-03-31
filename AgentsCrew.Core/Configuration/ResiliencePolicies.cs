using Polly;
using Polly.Retry;
using Polly.CircuitBreaker;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace AgentsCrew.Core.Configuration
{
    public static class ResiliencePolicies
    {
        public static AsyncRetryPolicy<T> GetDefaultRetryPolicy<T>(int maxRetries = 3, double baseDelaySeconds = 1.0)
        {
            return Policy<T>
                .Handle<HttpRequestException>()
                .Or<TimeoutException>()
                .Or<Exception>(ex => IsTransientException(ex))
                .WaitAndRetryAsync(
                    retryCount: maxRetries,
                    sleepDurationProvider: attempt => TimeSpan.FromSeconds(baseDelaySeconds * Math.Pow(2, attempt - 1)),
                    onRetry: (exception, timeSpan, retryCount, context) =>
                    {
                        Console.WriteLine($"[Polly] Retry {retryCount} due to {exception.Exception?.Message}. Waiting {timeSpan.TotalSeconds}s");
                    });
        }

        public static IAsyncPolicy<T> GetCompositePolicy<T>(int maxRetries = 3)
        {
            var retryPolicy = GetDefaultRetryPolicy<T>(maxRetries);
            return retryPolicy;
        }

        private static bool IsTransientException(Exception ex)
        {
            return ex is HttpRequestException ||
                   ex is TimeoutException ||
                   ex is System.Net.Sockets.SocketException ||
                   ex.Message.Contains("timeout", StringComparison.OrdinalIgnoreCase) ||
                   ex.Message.Contains("connection", StringComparison.OrdinalIgnoreCase);
        }
    }
}
