using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

using Benchmarks.Shared.Benchmarking;
using Benchmarks.Shared.Controls;

namespace Benchmarks.Shared.SuiteUI.ComplexUI.MultiplePropertyBench
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
                BorderBrush = new SolidColorBrush(Colors.Black),
                BorderThickness = new Thickness(10d),
                CornerRadius = new CornerRadius(0.5d),
                Padding = new Thickness(15d),

                Child = new TextBlock() { Text = "BORDERLINE" }
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