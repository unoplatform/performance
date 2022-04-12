using Benchmarks.Maui.Benchmarking;

namespace Benchmarks.Maui.SuiteUI
{
    internal class NoOpBenchmark : IAsyncUIBenchmark
    {
        public Task BenchmarkAsync()
        {
            return Task.CompletedTask;
        }
    }
}
