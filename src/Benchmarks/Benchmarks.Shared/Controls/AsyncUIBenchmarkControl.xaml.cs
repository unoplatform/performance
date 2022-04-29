using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

using Benchmarks.Shared.Benchmarking;

namespace Benchmarks.Shared.Controls
{
    public sealed partial class AsyncUIBenchmarkControl : UserControl
    {
        public AsyncUIBenchmarkControl()
        {
            InitializeComponent();
        }

        private void RunBenchmarks_Click(object sender, RoutedEventArgs e)
        {
            _ = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => await RunBenchmarks());
        }

        private IEnumerable<Type> EnumerateBenchmarks() =>
            GetType()
                .Assembly
                .GetTypes()
                .Where(t => !t.IsGenericType && t.GetInterfaces().Contains(typeof(IAsyncUIBenchmark)));

        private async Task RunBenchmarks()
        {
            try
            {
                AsyncUIBenchmarkHost.Root = FindName("UIHost") as ContentControl;

                Status.Text = "Running...";

                foreach (var benchmark in EnumerateBenchmarks())
                {
                    var times = new List<long>();

                    for (int x = 0; x < 10; x++)
                    {
                        var instance = (IAsyncUIBenchmark)Activator.CreateInstance(benchmark);
                        var setupTeardown = instance as IAsyncUIBenchmarkSetup;

                        await (setupTeardown?.SetupAsync() ?? Task.CompletedTask);

                        var sw = Stopwatch.StartNew();
                        await instance.BenchmarkAsync();
                        sw.Stop();

                        await (setupTeardown?.TeardownAsync() ?? Task.CompletedTask);

                        GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true, true);

                        times.Add(sw.ElapsedTicks);
                    }

                    WriteLog($"{benchmark.FullName}: Min {times.Min()} / Max {times.Max()} / Avg {times.Average()}");
                }

                Status.Text = "Finished.";
            }
            catch (Exception exception)
            {
                WriteLog(exception.ToString());

                Status.Text = "Failed.";
            }
            finally
            {
                AsyncUIBenchmarkHost.Root = null;
            }
        }

        private void WriteLog(string message)
        {
            Log.Inlines.Add(new Run() { Text = message });
            Log.Inlines.Add(new LineBreak());
        }
    }
}
