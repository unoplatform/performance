using System.Threading.Tasks;
using Benchmarks.Shared.Controls;
using Benchmarks.Maui.Benchmarking;
using Benchmarks.Maui.Util;

namespace Benchmarks.Maui.SuiteUI.ComplexUI.TreeBench
{
    internal class DeepTreeLoadedBenchmark : IAsyncUIBenchmark, IAsyncUIBenchmarkSetup
    {
        private StackLayout _sut;
        private TaskCompletionSource<bool> _tcs;
        private EventNotificationHandlerUtil _eventHandlerUtil = new EventNotificationHandlerUtil();


        public Task SetupAsync()
        {
            _eventHandlerUtil.Tcs = _tcs = new TaskCompletionSource<bool>();
            _sut = new StackLayout();
            _sut.ChildAdded += _eventHandlerUtil.ChildAddedHandler;

            _sut = MakePanel();

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

        private StackLayout MakePanel()
        {
            var current = new StackLayout()
            {
                Children = { new Entry() { Text = "DEEP TROUBLE !" } }
            };

            for (int x = 0; x < _eventHandlerUtil.MaxItems; x++)
            {
                var panel = new StackLayout();

                panel.Children.Add(current);

                _sut.Children.Add(panel);

                current = panel;
            }

            return current;
        }
    }
}