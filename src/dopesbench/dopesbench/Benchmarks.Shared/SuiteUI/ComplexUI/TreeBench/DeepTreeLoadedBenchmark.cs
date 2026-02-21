using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;

using Benchmarks.Shared.Benchmarking;
using Benchmarks.Shared.Controls;

namespace Benchmarks.Shared.SuiteUI.TreeBench
{
    internal class DeepTreeLoadedBenchmark : IAsyncUIBenchmark, IAsyncUIBenchmarkSetup
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
            var current = new StackPanel()
            {
                Children = { new TextBlock() { Text = "DEEP TROUBLE !" } }
            };
            
            for (int x = 0; x < 99; x++)
            {
                var panel = new StackPanel();

                panel.Children.Add(current);

                current = panel;
            }

            return current;
        }
    }
}
