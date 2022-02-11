using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

using Benchmarks.Shared.Benchmarking;
using Benchmarks.Shared.Controls;

namespace Benchmarks.Shared.SuiteUI.Windows_UI_Xaml_Controls.TextBlockBench
{
    internal class MultipleLoadedBenchmark : IAsyncUIBenchmark, IAsyncUIBenchmarkSetup
    {
        private StackPanel _sut;

        private TaskCompletionSource<bool> _tcs;

        public Task SetupAsync()
        {
            _tcs = new TaskCompletionSource<bool>();

            _sut = new StackPanel();

            for (int x = 0; x < 100; x++)
            {
                _sut.Children.Add(new TextBlock() { Text = "Hello Uno!" });
            }

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