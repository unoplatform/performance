using Microsoft.Maui.Controls.Shapes;

using Benchmarks.Maui.Benchmarking;
using Benchmarks.Maui.Controls;

namespace Benchmarks.Maui.SuiteUI.ComplexUI.MultiplePropertyBench
{
    internal class BorderLoadedBenchmark : IAsyncUIBenchmark, IAsyncUIBenchmarkSetup
    {
        private Border _sut;

        private TaskCompletionSource<bool> _tcs;

        public Task SetupAsync()
        {
            _tcs = new TaskCompletionSource<bool>();

            _sut = new Border()
            {
                Background = new SolidColorBrush(Colors.Yellow),
                Stroke = new SolidColorBrush(Colors.Black),
                StrokeThickness = 10d,
                StrokeShape = new RoundRectangle() { CornerRadius = new CornerRadius(20d) },
                Padding = new Thickness(15d),

                Content = new Label() { Text = "BORDERLINE" }
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