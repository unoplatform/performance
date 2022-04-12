using System.Diagnostics;

using Benchmarks.Maui.Benchmarking;

namespace Benchmarks.Maui.Controls
{
    public sealed partial class AsyncUIBenchmarkControl : ContentView
    {
        public AsyncUIBenchmarkControl()
        {
            InitializeComponent();
        }

        private void RunBenchmarks_Click(object sender, EventArgs e)
        {
            _ = Dispatcher.Dispatch(async () => await RunBenchmarks());
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
                AsyncUIBenchmarkHost.Root = UIHost;

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
            Log.Text += $"{message}\n";
        }
    }
}
