using Benchmarks.Maui.Benchmarking;

namespace Benchmarks.Maui.SuiteUI.ComplexUI.MultiplePropertyBench
{
    internal class TextBlockInitializeBenchmark : IAsyncUIBenchmark
    {
        public Task BenchmarkAsync()
        {
            var textBlock = new Entry()
            {
                FontFamily = "Currier New",
                FontSize = 20d,
                FontAttributes = FontAttributes.Italic,                
                TextColor = new Color(),
                HorizontalTextAlignment = TextAlignment.End,
                Text = "TYPo",                
                WidthRequest = 150d
            };

            return Task.CompletedTask;
        }
    }
}