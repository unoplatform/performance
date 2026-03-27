using System.Threading.Tasks;
using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

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
                FontStyle = Windows.UI.Text.FontStyle.Italic,
                FontWeight = FontWeights.ExtraBlack,
                Foreground = new SolidColorBrush(Colors.Red),
                HorizontalTextAlignment = TextAlignment.Right,
                Text = "TYPo",
                TextDecorations = Windows.UI.Text.TextDecorations.Strikethrough,
                Width = 150d
            };

            return Task.CompletedTask;
        }
    }
}
