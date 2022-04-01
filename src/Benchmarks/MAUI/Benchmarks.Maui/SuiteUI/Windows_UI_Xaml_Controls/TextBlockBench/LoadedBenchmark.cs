using Benchmarks.Shared.Controls;
using Benchmarks.Maui.Benchmarking;

namespace Benchmarks.Maui.SuiteUI.Windows_UI_Xaml_Controls.TextBlockBench
{
    internal class LoadedBenchmark : IAsyncUIBenchmark, IAsyncUIBenchmarkSetup
    {
        private Entry _sut;

        private TaskCompletionSource<bool> _tcs;

        public Task SetupAsync()
        {
            _tcs = new TaskCompletionSource<bool>();

            _sut = new Entry();
            _sut.PropertyChanged += this.PropertyChangedHandler;

            _sut.Text = "Hello Uno!";

            return Task.CompletedTask;
        }

        private void PropertyChangedHandler(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Entry.TextProperty.PropertyName)
            {
                _tcs.SetResult(true);

                var entry = (Entry)sender;

                entry.PropertyChanged -= this.PropertyChangedHandler;
            }
        }

        public Task BenchmarkAsync()
        {
            AsyncUIBenchmarkHost.Root.Children.Add(_sut);

            return _tcs.Task;
        }

        public Task TeardownAsync()
        {
            AsyncUIBenchmarkHost.Root.RemoveAt(0);

            return Task.CompletedTask;
        }
    }
}