using Benchmarks.Maui.Benchmarking;  

namespace Benchmarks.Maui.SuiteUI.ComplexUI.FormBench
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