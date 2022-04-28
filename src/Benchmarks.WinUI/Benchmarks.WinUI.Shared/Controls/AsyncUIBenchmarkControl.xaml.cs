using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;

using Benchmarks.WinUI.Shared.Benchmarking;

namespace Benchmarks.WinUI.Shared.Controls
{
    public sealed partial class AsyncUIBenchmarkControl : UserControl
    {
        public AsyncUIBenchmarkControl()
        {
            InitializeComponent();
        }

        private void RunBenchmarks_Click(object sender, RoutedEventArgs e)
        {
            _ = DispatcherQueue.TryEnqueue(DispatcherQueuePriority.Normal, async () => await RunBenchmarks());
        }

        private IEnumerable<Type> EnumerateBenchmarks() =>
            GetType()
                .Assembly
                .GetTypes()
                .Where(t => !t.IsGenericType && t.GetInterfaces().Contains(typeof(IAsyncUIBenchmark)))
                .OrderBy(t => t.FullName);

        private async Task RunBenchmarks()
        {
            try
            {
                AsyncUIBenchmarkHost.Root = FindName("UIHost") as ContentControl;

                Status.Text = "Running...";

                WriteLog($"Name;Min (ms);Max (ms);Avg (ms)");

                foreach (var benchmark in EnumerateBenchmarks())
                {
                    var times = new List<double>();

                    // Warmup
                    await RunTest(benchmark);

                    for (int x = 0; x < 10; x++)
                    {
                        var result = await RunTest(benchmark);

                        times.Add(result.TotalMilliseconds);
                    }

                    WriteLog($"{benchmark.FullName.Replace("Benchmarks.WinUI.Shared.SuiteUI.", string.Empty)};{times.Min():0.000000};{times.Max():0.000000};{times.Average():0.000000}");
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

            static async Task<TimeSpan> RunTest(Type benchmark)
            {
                var instance = (IAsyncUIBenchmark)Activator.CreateInstance(benchmark);
                var setupTeardown = instance as IAsyncUIBenchmarkSetup;

                await (setupTeardown?.SetupAsync() ?? Task.CompletedTask);

                var sw = Stopwatch.StartNew();
                await instance.BenchmarkAsync();
                sw.Stop();

                await (setupTeardown?.TeardownAsync() ?? Task.CompletedTask);

                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true, true);
                
                return sw.Elapsed;
            }
        }

        private void WriteLog(string message)
        {
            Log.Text += $"{message}\n";
        }
    }
}
