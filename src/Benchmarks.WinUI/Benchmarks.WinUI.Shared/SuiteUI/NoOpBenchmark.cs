using System.Threading.Tasks;

using Benchmarks.WinUI.Shared.Benchmarking;

namespace Benchmarks.WinUI.Shared.SuiteUI
{
    internal class NoOpBenchmark : IAsyncUIBenchmark
    {
        public Task BenchmarkAsync()
        {
            return Task.CompletedTask;
        }
    }
}
