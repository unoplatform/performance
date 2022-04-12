using Benchmarks.Maui.Benchmarking;
using Benchmarks.Maui.Controls;

namespace Benchmarks.Maui.SuiteUI.Microsoft_Maui_Controls.LabelBench
{
    internal class MultipleLoadedBenchmark : IAsyncUIBenchmark, IAsyncUIBenchmarkSetup
    {
        private StackLayout _sut;

        private TaskCompletionSource<bool> _tcs;

        public Task SetupAsync()
        {
            _tcs = new TaskCompletionSource<bool>();

            _sut = new StackLayout();

            for (int x = 0; x < 100; x++)
            {
                _sut.Children.Add(new Label() { Text = "Hello Uno!" });
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