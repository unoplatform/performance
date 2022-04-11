using System.Threading.Tasks;

using Benchmarks.WinUI.Shared.Benchmarking;

namespace Benchmarks.WinUI.Shared.SuiteUI.ComplexUI.FormBench
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