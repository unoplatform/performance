using System.Threading.Tasks;
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
                Padding = new Thickness(15d),

                Content = new Entry() { Text = "BORDERLINE" }
            };

            return Task.CompletedTask;
        }
    }
}