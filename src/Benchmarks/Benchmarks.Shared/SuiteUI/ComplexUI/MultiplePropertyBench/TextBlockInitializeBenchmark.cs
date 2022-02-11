using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

using Benchmarks.Shared.Benchmarking;

namespace Benchmarks.Shared.SuiteUI.ComplexUI.MultiplePropertyBench
{
    internal class TextBlockInitializeBenchmark : IAsyncUIBenchmark
    {
        public Task BenchmarkAsync()
        {
            var textBlock = new TextBlock()
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

            return Task.CompletedTask;
        }
    }
}