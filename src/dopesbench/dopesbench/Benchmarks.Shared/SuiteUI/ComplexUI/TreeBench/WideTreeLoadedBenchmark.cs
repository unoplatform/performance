using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;

using Benchmarks.Shared.Benchmarking;
using Benchmarks.Shared.Controls;

namespace Benchmarks.Shared.SuiteUI.TreeBench
{
    internal class WideTreeLoadedBenchmark : IAsyncUIBenchmark, IAsyncUIBenchmarkSetup
    {
        private StackPanel _sut;

        private TaskCompletionSource<bool> _tcs;

        public Task SetupAsync()
        {
            _tcs = new TaskCompletionSource<bool>();

            _sut = MakePanel();
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

        private StackPanel MakePanel()
        {
            var panel = new StackPanel();

            for (int x = 0; x < 50; x++)
            {
                var current = new StackPanel()
                {
                    Children = { new TextBlock() { Text = "WIDE BERTH !" } }
                };

                panel.Children.Add(current);
            }

            return panel;
        }
    }
}
