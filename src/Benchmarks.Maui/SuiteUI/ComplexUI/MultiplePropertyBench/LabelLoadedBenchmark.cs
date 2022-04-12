using Benchmarks.Maui.Benchmarking;
using Benchmarks.Maui.Controls;

namespace Benchmarks.Maui.SuiteUI.ComplexUI.MultiplePropertyBench
{
    internal class LabelLoadedBenchmark : IAsyncUIBenchmark, IAsyncUIBenchmarkSetup
    {
        private Label _sut;

        private TaskCompletionSource<bool> _tcs;

        public Task SetupAsync()
        {
            _tcs = new TaskCompletionSource<bool>();

            _sut = new Label()
            {
                FontFamily = "Courier New",
                FontSize = 20d,
                FontAttributes = FontAttributes.Bold | FontAttributes.Italic,
                TextColor = Colors.Red,
                HorizontalTextAlignment = TextAlignment.End,
                Text = "TYPo",
                TextDecorations = TextDecorations.Strikethrough,
                WidthRequest = 150d
            };
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