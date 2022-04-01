using Benchmarks.Shared.Controls;
using Benchmarks.Maui.Benchmarking;

namespace Benchmarks.Maui.SuiteUI.ComplexUI.FormBench
{
    internal class FormLoadedBenchmark : IAsyncUIBenchmark, IAsyncUIBenchmarkSetup
    {
        private Grid _sut;

        private TaskCompletionSource<bool> _tcs;

        public Task SetupAsync()
        {
            _tcs = new TaskCompletionSource<bool>();

            _sut = FormBenchHelper.MakeGrid();
            _tcs.SetResult(true);

            return Task.CompletedTask;
        }

        public Task BenchmarkAsync()
        {
            AsyncUIBenchmarkHost.Root.Children.Add(_sut);

            return _tcs.Task;
        }

        public Task TeardownAsync()
        {
            AsyncUIBenchmarkHost.Root.Children.RemoveAt(0);

            return Task.CompletedTask;
        }
    }
}