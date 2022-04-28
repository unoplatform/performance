namespace Benchmarks.Maui.Controls
{
	internal class AsyncUIBenchmarkHost
	{
		public static ContentView Root { get; set; }

		internal static void WaitForIdle(BindableObject bindable, Action action) => bindable.Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(1d), action);
	}
}
