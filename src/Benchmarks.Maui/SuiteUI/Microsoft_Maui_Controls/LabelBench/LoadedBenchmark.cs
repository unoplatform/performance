using Benchmarks.Maui.Benchmarking;
using Benchmarks.Maui.Controls;

namespace Benchmarks.Maui.SuiteUI.Microsoft_Maui_Controls.LabelBench
{
    internal class LoadedBenchmark : IAsyncUIBenchmark, IAsyncUIBenchmarkSetup
    {
        private Label _sut;

        private TaskCompletionSource<bool> _tcs;

        public Task SetupAsync()
        {
            _tcs = new TaskCompletionSource<bool>();

            _sut = new Label();
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