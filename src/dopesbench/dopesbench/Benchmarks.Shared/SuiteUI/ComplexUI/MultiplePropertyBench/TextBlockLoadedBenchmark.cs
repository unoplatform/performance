using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

using Benchmarks.Shared.Benchmarking;
using Benchmarks.Shared.Controls;
using Microsoft.UI;
using Microsoft.UI.Text;

namespace Benchmarks.Shared.SuiteUI.ComplexUI.MultiplePropertyBench
{
    internal class TextBlockLoadedBenchmark : IAsyncUIBenchmark, IAsyncUIBenchmarkSetup
    {
        private TextBlock _sut;

        private TaskCompletionSource<bool> _tcs;

        public Task SetupAsync()
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
