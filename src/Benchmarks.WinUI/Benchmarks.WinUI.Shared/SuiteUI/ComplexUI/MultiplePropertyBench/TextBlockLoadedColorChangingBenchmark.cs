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
    internal class TextBlockLoadedColorChangingBenchmark : IAsyncUIBenchmark, IAsyncUIBenchmarkSetup
    {
        private TextBlock _sut;
        private TaskCompletionSource<bool> _tcs;

        private Brush _color1 = new SolidColorBrush(ColorHelper.FromArgb(0, 0, 0, 0));
        private Brush _color2 = new SolidColorBrush(ColorHelper.FromArgb(2, 2, 2, 2));

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
                _sut.Foreground = (i % 2) == 0 ? _color1 : _color2;
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