using System.Threading.Tasks;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

using Benchmarks.WinUI.Shared.Benchmarking;
using Benchmarks.WinUI.Shared.Controls;

namespace Benchmarks.WinUI.Shared.SuiteUI.ComplexUI.MultiplePropertyBench
{
    internal class BorderInitializeAndLoadBenchmark : IAsyncUIBenchmark, IAsyncUIBenchmarkSetup
    {
        private TaskCompletionSource<bool> _tcs;

        public Task SetupAsync()
        {
            _tcs = new TaskCompletionSource<bool>();

            return Task.CompletedTask;
        }

        public Task BenchmarkAsync()
        {
            var sut = new Border()
            {
                Background = new SolidColorBrush(Colors.Yellow),
                BorderBrush = new SolidColorBrush(Colors.Black),
                BorderThickness = new Thickness(10d),
                CornerRadius = new CornerRadius(0.5d),
                Padding = new Thickness(15d),

                Child = new TextBlock() { Text = "BORDERLINE" }
            };

            AsyncUIBenchmarkHost.WaitForIdle(sut, () => _tcs.SetResult(true));

            AsyncUIBenchmarkHost.Root.Content = sut;

            return _tcs.Task;
        }

        public Task TeardownAsync()
        {
            AsyncUIBenchmarkHost.Root.Content = null;

            return Task.CompletedTask;
        }
    }
}