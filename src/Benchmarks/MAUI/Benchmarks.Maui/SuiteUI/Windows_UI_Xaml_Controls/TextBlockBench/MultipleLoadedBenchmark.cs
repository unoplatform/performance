using Benchmarks.Shared.Controls;
using Benchmarks.Maui.Benchmarking;
using Benchmarks.Maui.Util;

namespace Benchmarks.Maui.SuiteUI.Windows_UI_Xaml_Controls.TextBlockBench
{
    internal class MultipleLoadedBenchmark : IAsyncUIBenchmark, IAsyncUIBenchmarkSetup
    {
        private StackLayout _sut;
        private TaskCompletionSource<bool> _tcs;
        private EventNotificationHandlerUtil _eventHandlerUtil = new EventNotificationHandlerUtil();


        public Task SetupAsync()
        {
            _eventHandlerUtil.Tcs = _tcs = new TaskCompletionSource<bool>();

            _sut = new StackLayout();
            _sut.ChildAdded += _eventHandlerUtil.ChildAddedHandler;

            for (int x = 0; x < _eventHandlerUtil.MaxItems; x++)
            {
                _sut.Children.Add(new Entry() { Text = "Hello Uno!" });
            }

            return Task.CompletedTask;
        }

        public Task BenchmarkAsync()
        {
            AsyncUIBenchmarkHost.Root.Children.Add(_sut);

            return _tcs.Task;
        }

        public Task TeardownAsync()
        {
            AsyncUIBenchmarkHost.Root.RemoveAt(0);

            return Task.CompletedTask;
        }
    }
}