using System.Threading.Tasks;

using Benchmarks.Shared.Benchmarking;

namespace Benchmarks.Shared.SuiteUI.ComplexUI.FormBench
{
    internal class FormInitializeBenchmark : IAsyncUIBenchmark
    {
        public Task BenchmarkAsync()
        {
            FormBenchHelper.MakeGrid();

            return Task.CompletedTask;
        }
    }
}