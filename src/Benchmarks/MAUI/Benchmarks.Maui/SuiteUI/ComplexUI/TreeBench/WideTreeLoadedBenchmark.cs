using System.Threading.Tasks;
using Benchmarks.Shared.Controls;
using Benchmarks.Maui.Benchmarking;
using Benchmarks.Maui.Util;

namespace Benchmarks.Maui.SuiteUI.ComplexUI.TreeBench
{
    internal class WideTreeLoadedBenchmark : IAsyncUIBenchmark, IAsyncUIBenchmarkSetup
    {
        private StackLayout _sut;
        private TaskCompletionSource<bool> _tcs;
        private EventNotificationHandlerUtil _eventHandlerUtil = new EventNotificationHandlerUtil();

        public Task SetupAsync()
        {
            _sut = new StackLayout(); 
            _eventHandlerUtil .Tcs = _tcs = new TaskCompletionSource<bool>();
            _eventHandlerUtil.MaxItems = 50;
            _sut.ChildAdded +=  _eventHandlerUtil.ChildAddedHandler;
            _sut = MakePanel();
            

            return Task.CompletedTask;
        }

        public Task BenchmarkAsync()
        {
            AsyncUIBenchmarkHost.Root.Add(_sut);

            return _tcs.Task;
        }

        public Task TeardownAsync()
        {
            AsyncUIBenchmarkHost.Root.RemoveAt(0);

            return Task.CompletedTask;
        }

        private StackLayout MakePanel()
        {
            var panel = new StackLayout();

            for (int x = 0; x < 50; x++)
            {
                var current = new StackLayout()
                {
                    Children = { new Entry() { Text = "WIDE BERTH !" } }
                };

                panel.Children.Add(current);

                _sut.Children.Add(panel);
            }

            return panel;
        }
    }
}