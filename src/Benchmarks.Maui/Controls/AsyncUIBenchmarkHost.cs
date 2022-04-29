namespace Benchmarks.Maui.Controls
{
	internal class AsyncUIBenchmarkHost
	{
		public static ContentView Root { get; set; }

        internal static void WaitForIdle(BindableObject bindable, Action action)
        {
            bindable.Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(.1), action);
        }

        internal static async Task WaitForIdleAsync(BindableObject bindable)
        {
            var tcs = new TaskCompletionSource<bool>();
            bindable.Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(.1), () => tcs.TrySetResult(true));
            await tcs.Task;
        }
    }
}
