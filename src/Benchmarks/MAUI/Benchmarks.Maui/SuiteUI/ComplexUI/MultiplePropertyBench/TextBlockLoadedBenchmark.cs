using Benchmarks.Shared.Controls;
using Benchmarks.Maui.Benchmarking;

namespace Benchmarks.Maui.SuiteUI.ComplexUI.MultiplePropertyBench
{
    internal class TextBlockLoadedBenchmark : IAsyncUIBenchmark, IAsyncUIBenchmarkSetup
    {
        private Entry _sut = new Entry();

        private TaskCompletionSource<bool> _tcs;

        public Task SetupAsync()
        {
            _tcs = new TaskCompletionSource<bool>();            
            _sut = new Entry()
            {   
                FontSize = 20d,
                TextColor = new Color(),
                HorizontalTextAlignment = TextAlignment.End,                
                WidthRequest = 150d,
            };

            _sut.Text = "typo";

            _tcs.TrySetResult(true);

            return Task.CompletedTask;
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