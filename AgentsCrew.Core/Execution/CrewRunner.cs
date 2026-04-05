using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AgentsCrew.Core.Interfaces;

namespace AgentsCrew.Core.Execution
{
    public class CrewRunner
    {
        private readonly ICrew _crew;

        public CrewRunner(ICrew crew)
        {
            _crew = crew ?? throw new ArgumentNullException(nameof(crew));
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            // Oportunidad futura: Integrar un ILogger aquí si _crew.Verbose > 0
            if (_crew.Verbose > 0)
            {
                Console.WriteLine($"[CrewRunner] Starting Crew Execution. Agents: {_crew.Agents.Count}, Tasks: {_crew.Tasks.Count}");
            }

            try
            {
                // Delegar al Process (Sequential o Hierarchical)
                await _crew.Process.ExecuteAsync(_crew.Agents, _crew.Tasks, _crew.MemoryContext);

                if (_crew.Verbose > 0)
                {
                    Console.WriteLine("[CrewRunner] Crew Execution completed successfully.");
                    PrintTelemetrySummary();
                }
            }
            catch (Exception ex)
            {
                if (_crew.Verbose > 0)
                {
                    Console.WriteLine($"[CrewRunner] Crew Execution failed: {ex.Message}");
                }
                throw;
            }
        }

        private void PrintTelemetrySummary()
        {
            long totalTokens = 0;
            long totalTimeMs = 0;

            foreach (var agent in _crew.Agents)
            {
                if (agent.Metrics != null)
                {
                    totalTokens += agent.Metrics.TotalTokensUsed;
                    totalTimeMs += (long)agent.Metrics.TotalExecutionTime.TotalMilliseconds;
                }
            }

            Console.WriteLine("\n--- 📊 CREW TELEMETRY SUMMARY ---");
            Console.WriteLine($"Total Tokens Used (Estimated): {totalTokens}");
            Console.WriteLine($"Total Execution Time (LLM ms): {totalTimeMs}");
            Console.WriteLine("---------------------------------\n");
        }
    }
}
