using System.Threading.Tasks;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

using Benchmarks.WinUI.Shared.Benchmarking;
using Benchmarks.WinUI.Shared.Controls;

namespace Benchmarks.WinUI.Shared.SuiteUI.ComplexUI.MultiplePropertyBench
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