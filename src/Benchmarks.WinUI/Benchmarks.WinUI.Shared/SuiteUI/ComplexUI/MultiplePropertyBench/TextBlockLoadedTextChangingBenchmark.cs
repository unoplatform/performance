using System.Threading.Tasks;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.UI.Text;

using Benchmarks.WinUI.Shared.Benchmarking;
using Benchmarks.WinUI.Shared.Controls;

using FontWeights = Microsoft.UI.Text.FontWeights;
using System;

namespace Benchmarks.WinUI.Shared.SuiteUI.ComplexUI.MultiplePropertyBench
{
    internal class TextBlockLoadedTextChangingBenchmark : IAsyncUIBenchmark, IAsyncUIBenchmarkSetup
    {
        private TextBlock _sut;

        private TaskCompletionSource<bool> _tcs;

        public async Task SetupAsync()
        {
            _tcs = new TaskCompletionSource<bool>();

            _sut = new TextBlock()
            {
                FontFamily = new FontFamily("Courier New"),
                FontSize = 20d,
                FontStyle = FontStyle.Italic,
                FontWeight = FontWeights.ExtraBlack,
                Foreground = new SolidColorBrush(Colors.Red),
                HorizontalTextAlignment = TextAlignment.Right,
                Text = "TYPo",
                TextDecorations = TextDecorations.Strikethrough,
                Width = 150d
            };

            AsyncUIBenchmarkHost.WaitForIdle(_sut, () => _tcs.SetResult(true));

            AsyncUIBenchmarkHost.Root.Content = _sut;

            await _tcs.Task;
        }

        public async Task BenchmarkAsync()
        {
            for (int i = 0; i < 100; i++)
            {
                _sut.Text = i.ToString();
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