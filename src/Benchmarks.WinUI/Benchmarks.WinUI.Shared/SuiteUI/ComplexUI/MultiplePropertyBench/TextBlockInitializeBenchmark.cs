using System.Threading.Tasks;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.UI.Text;

using Benchmarks.WinUI.Shared.Benchmarking;

using FontWeights = Microsoft.UI.Text.FontWeights;

namespace Benchmarks.WinUI.Shared.SuiteUI.ComplexUI.MultiplePropertyBench
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