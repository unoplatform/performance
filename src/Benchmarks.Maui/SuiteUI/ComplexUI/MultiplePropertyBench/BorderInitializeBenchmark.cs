using Microsoft.Maui.Controls.Shapes;

using Benchmarks.Maui.Benchmarking;

namespace Benchmarks.Maui.SuiteUI.ComplexUI.MultiplePropertyBench
{
    internal class BorderInitializeBenchmark : IAsyncUIBenchmark
    {
        public Task BenchmarkAsync()
        {
            var border = new Border()
            {
                Background = new SolidColorBrush(Colors.Yellow),
                Stroke = new SolidColorBrush(Colors.Black),
                StrokeThickness = 10d,
                StrokeShape = new RoundRectangle() { CornerRadius = new CornerRadius(20d) },
                Padding = new Thickness(15d),
                
                Content = new Label() { Text = "BORDERLINE" }
            };

            return Task.CompletedTask;
        }
    }
}