using Benchmarks.Maui.Benchmarking;
using Benchmarks.Maui.Controls;

namespace Benchmarks.Maui.SuiteUI.ComplexUI.MultiplePropertyBench
{
    internal class TextBlockLoadedColorChangingBenchmark : IAsyncUIBenchmark, IAsyncUIBenchmarkSetup
    {
        private Label _sut;

        private TaskCompletionSource<bool> _tcs;

        public async Task SetupAsync()
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
            AsyncUIBenchmarkHost.WaitForIdle(_sut, () => _tcs.SetResult(true));
            AsyncUIBenchmarkHost.Root.Content = _sut;

            await _tcs.Task;
        }

        public async Task BenchmarkAsync()
        {
            for (int i = 0; i < 100; i++)
            {
                _sut.TextColor = new Color(i);

                await AsyncUIBenchmarkHost.WaitForIdleAsync(_sut);
            }
        }

        public Task TeardownAsync()
        {
            AsyncUIBenchmarkHost.Root.Content = null;

            return Task.CompletedTask;
        }
    }
}