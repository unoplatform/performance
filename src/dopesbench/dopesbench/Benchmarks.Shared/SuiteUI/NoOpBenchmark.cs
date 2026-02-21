using System.Threading.Tasks;

using Benchmarks.Shared.Benchmarking;

namespace Benchmarks.Shared.SuiteUI
{
    internal class NoOpBenchmark : IAsyncUIBenchmark
    {
        public Task BenchmarkAsync()
        {
            return Task.CompletedTask;
        }
    }
}
