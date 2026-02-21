using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;

using Benchmarks.Shared.Benchmarking;
using Benchmarks.Shared.Controls;

namespace Benchmarks.Shared.SuiteUI.ComplexUI.FormBench
{
    internal class FormLoadedBenchmark : IAsyncUIBenchmark, IAsyncUIBenchmarkSetup
    {
        private Grid _sut;

        private TaskCompletionSource<bool> _tcs;

        public Task SetupAsync()
        {
            _tcs = new TaskCompletionSource<bool>();

            _sut = FormBenchHelper.MakeGrid();
            _sut.Loaded += (s, e) => _tcs.SetResult(true);

            return Task.CompletedTask;
        }

        public Task BenchmarkAsync()
        {
            AsyncUIBenchmarkHost.Root.Content = _sut;

            return _tcs.Task;
        }

        public Task TeardownAsync()
        {
            AsyncUIBenchmarkHost.Root.Content = null;

            return Task.CompletedTask;
        }
    }
}
