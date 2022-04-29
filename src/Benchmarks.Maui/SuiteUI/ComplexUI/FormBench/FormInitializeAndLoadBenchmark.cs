using Benchmarks.Maui.Benchmarking;
using Benchmarks.Maui.Controls;

namespace Benchmarks.Maui.SuiteUI.ComplexUI.FormBench
{
    internal class FormInitializeAndLoadBenchmark : IAsyncUIBenchmark, IAsyncUIBenchmarkSetup
    {
        private TaskCompletionSource<bool> _tcs;

        public Task SetupAsync()
        {
            _tcs = new TaskCompletionSource<bool>();

            return Task.CompletedTask;
        }

        public Task BenchmarkAsync()
        {
            var sut = FormBenchHelper.MakeGrid();

            AsyncUIBenchmarkHost.WaitForIdle(sut, () => _tcs.SetResult(true));

            AsyncUIBenchmarkHost.Root.Content = sut;

            return _tcs.Task;
        }

        public Task TeardownAsync()
        {
            AsyncUIBenchmarkHost.Root.Content = null;

            return Task.CompletedTask;
        }
    }
}