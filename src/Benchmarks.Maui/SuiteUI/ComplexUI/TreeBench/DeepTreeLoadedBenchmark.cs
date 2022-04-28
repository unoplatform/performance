using Benchmarks.Maui.Benchmarking;
using Benchmarks.Maui.Controls;

namespace Benchmarks.Maui.SuiteUI.TreeBench
{
    internal class DeepTreeLoadedBenchmark : IAsyncUIBenchmark, IAsyncUIBenchmarkSetup
    {
        private StackLayout _sut;

        private TaskCompletionSource<bool> _tcs;

        public Task SetupAsync()
        {
            _tcs = new TaskCompletionSource<bool>();

            _sut = MakePanel();

            AsyncUIBenchmarkHost.WaitForIdle(_sut, () => _tcs.SetResult(true));

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
            var current = new StackLayout()
            {
                Children = { new Label() { Text = "DEEP TROUBLE !" } }
            };
            
            for (int x = 0; x < 99; x++)
            {
                var panel = new StackLayout();

                panel.Children.Add(current);

                current = panel;
            }

            return current;
        }
    }
}