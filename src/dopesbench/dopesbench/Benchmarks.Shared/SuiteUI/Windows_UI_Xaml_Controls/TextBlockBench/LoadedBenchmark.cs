using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;

using Benchmarks.Shared.Benchmarking;
using Benchmarks.Shared.Controls;

namespace Benchmarks.Shared.SuiteUI.Windows_UI_Xaml_Controls.TextBlockBench
{
    internal class LoadedBenchmark : IAsyncUIBenchmark, IAsyncUIBenchmarkSetup
    {
        private TextBlock _sut;

        private TaskCompletionSource<bool> _tcs;

        public Task SetupAsync()
        {
            _tcs = new TaskCompletionSource<bool>();

            _sut = new TextBlock();
            _sut.Text = "Hello Uno!";
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
