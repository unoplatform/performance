using System.Threading.Tasks;
using Benchmarks.Shared.Controls;
using Benchmarks.Maui.Benchmarking;

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
                Padding = new Thickness(15d),
            };

            _sut.PropertyChanged += (sender,  e) =>
            {
                if (e.PropertyName == Border.ContentProperty.PropertyName)
                {
                    _tcs.SetResult(true);
                }
            };

            _sut.Content = new Entry() { Text = "BORDERLINE" };

            return Task.CompletedTask;
        }

        public Task BenchmarkAsync()
        {   
            AsyncUIBenchmarkHost.Root.Children.Add(_sut);

            return _tcs.Task;
        }

        public Task TeardownAsync()
        {
            AsyncUIBenchmarkHost.Root.Children.RemoveAt(0);

            return Task.CompletedTask;
        }
    }
}