using Benchmarks.Maui.Benchmarking;
using Benchmarks.Maui.Controls;

namespace Benchmarks.Maui.SuiteUI.TreeBench
{
    internal class WideTreeLoadedBenchmark : IAsyncUIBenchmark, IAsyncUIBenchmarkSetup
    {
        private StackLayout _sut;

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

        private StackLayout MakePanel()
        {
            var panel = new StackLayout();

            for (int x = 0; x < 50; x++)
            {
                var current = new StackLayout()
                {
                    Children = { new Label() { Text = "WIDE BERTH !" } }
                };

                panel.Children.Add(current);
            }

            return panel;
        }
    }
}