using Benchmarks.Maui.Benchmarking;
using Benchmarks.Maui.Model;
using Benchmarks.Shared.Controls;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;

namespace Benchmarks.Maui.Controls;

public partial class AsyncUIBenchmarkControl : Grid
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
            Logs.Clear();
            AsyncUIBenchmarkHost.Root = UIHost;

            Status.Text = "Running...";

            var bechemarks = EnumerateBenchmarks();


            int totalBenchs = bechemarks.Count();
            int count = 0;

            foreach (var benchmark in bechemarks)
            {
                count++;


                var times = new List<long>();                

                Dispatcher.Dispatch(() => Status.Text = $"Benchmark started..{benchmark.Name} --> {count} of {totalBenchs}");
                await Task.Delay(50);

                for (int x = 0; x < 10; x++)
                {
                    var instance = (IAsyncUIBenchmark)Activator.CreateInstance(benchmark);
                    var setupTeardown = instance as IAsyncUIBenchmarkSetup;

                    await (setupTeardown?.SetupAsync() ?? Task.CompletedTask);

                    var sw = Stopwatch.StartNew();
                    await instance.BenchmarkAsync();
                    sw.Stop();

                    await (setupTeardown?.TeardownAsync() ?? Task.CompletedTask);

                    times.Add(sw.ElapsedTicks);
                }
                                
                WriteLog($"{benchmark.Name}: Min {times.Min()} / Max {times.Max()} / Avg {times.Average()}");
                await Task.Delay(25);
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
        Dispatcher.Dispatch(() =>
        {

            Logs.Add(new LogInfo() { LogText = message });
            Log.ItemsSource = Logs;
        }
        );        
    }

    private ObservableCollection<LogInfo> Logs { get; set; } = new ObservableCollection<LogInfo>();


}

