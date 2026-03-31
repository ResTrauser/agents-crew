using System;
using System.Threading;

namespace AgentsCrew.Core.Telemetry
{
    public class UsageMetrics
    {
        private long _totalTokensUsed;
        private long _totalExecutionTimeMs;

        public long TotalTokensUsed => Interlocked.Read(ref _totalTokensUsed);
        public TimeSpan TotalExecutionTime => TimeSpan.FromMilliseconds(Interlocked.Read(ref _totalExecutionTimeMs));

        public void AddTokens(long tokenCount)
        {
            Interlocked.Add(ref _totalTokensUsed, tokenCount);
        }

        public void AddExecutionTime(long executionTimeMs)
        {
            Interlocked.Add(ref _totalExecutionTimeMs, executionTimeMs);
        }

        public override string ToString()
        {
            return $"--- CrewAI Telemetry ---\nTotal Tokens Used: {TotalTokensUsed}\nTotal Execution Time: {TotalExecutionTime.TotalSeconds} seconds";
        }
    }
}
