using Benchmarks.Maui.Benchmarking;
using Benchmarks.Maui.Controls;

namespace Benchmarks.Maui.SuiteUI.TreeBench
{
    internal class GridDeepResizeBenchmark : IAsyncUIBenchmark, IAsyncUIBenchmarkSetup
    {
        private Grid _sut;
        private TaskCompletionSource<bool> _tcs;
        private Grid _innerMost;

        public Task SetupAsync()
        {
            _tcs = new TaskCompletionSource<bool>();

            _innerMost = new Grid() { WidthRequest = 100, HeightRequest = 100 };
            _sut = MakePanel();

            AsyncUIBenchmarkHost.WaitForIdle(_sut, () => _tcs.SetResult(true));

            AsyncUIBenchmarkHost.Root.Content = _sut;

            return _tcs.Task;
        }

        public async Task BenchmarkAsync()
        {
            for (int i = 0; i < 10; i++)
            {
                _innerMost.WidthRequest += 50;
                await AsyncUIBenchmarkHost.WaitForIdleAsync(_sut);
            }
        }

        public Task TeardownAsync()
        {
            AsyncUIBenchmarkHost.Root.Content = null;

            return Task.CompletedTask;
        }

        private Grid MakePanel()
        {
            var panel = new Grid();

            var inner = panel;
            for (int x = 0; x < 50; x++)
            {
                var child = new Grid();

                child.ColumnDefinitions.Add(new ColumnDefinition());
                child.ColumnDefinitions.Add(new ColumnDefinition());
                child.RowDefinitions.Add(new RowDefinition());
                child.RowDefinitions.Add(new RowDefinition());

                inner.Children.Add(child);

                inner = child;
            }

            inner.Children.Add(_innerMost);

            return panel;
        }
    }
}