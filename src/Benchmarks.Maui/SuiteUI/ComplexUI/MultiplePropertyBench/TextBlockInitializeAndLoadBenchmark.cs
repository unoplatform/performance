using Benchmarks.Maui.Benchmarking;
using Benchmarks.Maui.Controls;

namespace Benchmarks.Maui.SuiteUI.ComplexUI.MultiplePropertyBench
{
    internal class TextBlockInitializeAndLoadBenchmark : IAsyncUIBenchmark, IAsyncUIBenchmarkSetup
    {
        private TaskCompletionSource<bool> _tcs;

        public Task SetupAsync()
        {
            _tcs = new TaskCompletionSource<bool>();

            return Task.CompletedTask;
        }

        public Task BenchmarkAsync()
        {
            var sut = new Label()
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

            AsyncUIBenchmarkHost.WaitForIdle(sut, () => _tcs.SetResult(true));

            AsyncUIBenchmarkHost.Root.Content = sut;

            return _tcs.Task;
        }

        public Task TeardownAsync()
        {
            AsyncUIBenchmarkHost.Root.Content = null;

            return Task.CompletedTask;
        }
    }
}