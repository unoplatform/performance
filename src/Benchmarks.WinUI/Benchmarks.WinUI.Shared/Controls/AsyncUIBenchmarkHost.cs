using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;

namespace Benchmarks.WinUI.Shared.Controls
{
	internal class AsyncUIBenchmarkHost
	{
		public static ContentControl Root { get; set; }

        internal static void WaitForIdle(UIElement element, Action action)
        {
            var timer = element.DispatcherQueue.CreateTimer();
            timer.Interval = TimeSpan.FromMilliseconds(.1);
            timer.Tick += OnTimerTick;
            timer.Start();

            void OnTimerTick(DispatcherQueueTimer sender, object args)
            {
                action();

#if !WINDOWS
                // Stopping a timer on windows is very expensive
                timer.Stop();
#endif

                timer.Tick -= OnTimerTick;
            }
        }

        internal static async Task WaitForIdleAsync(UIElement element)
        {
            var tcs = new TaskCompletionSource<bool>();
            WaitForIdle(element, () => tcs.TrySetResult(true));
            await tcs.Task;
        }
    }
}
