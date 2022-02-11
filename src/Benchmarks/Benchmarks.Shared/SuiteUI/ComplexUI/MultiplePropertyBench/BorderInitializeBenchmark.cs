using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

using Benchmarks.Shared.Benchmarking;

namespace Benchmarks.Shared.SuiteUI.ComplexUI.MultiplePropertyBench
{
    internal class BorderInitializeBenchmark : IAsyncUIBenchmark
    {
        public Task BenchmarkAsync()
        {
            var border = new Border()
            {
                Background = new SolidColorBrush(Colors.Yellow),
                BorderBrush = new SolidColorBrush(Colors.Black),
                BorderThickness = new Thickness(10d),
                CornerRadius = new CornerRadius(0.5d),
                Padding = new Thickness(15d),
                
                Child = new TextBlock() { Text = "BORDERLINE" }
            };

            return Task.CompletedTask;
        }
    }
}