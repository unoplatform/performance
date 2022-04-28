using System;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Benchmarks.WinUI.Shared.Controls
{
	internal class AsyncUIBenchmarkHost
	{
		public static ContentControl Root { get; set; }

		internal static void WaitForIdle(UIElement element, Action action)
		{
			var timer = element.DispatcherQueue.CreateTimer();
			timer.Interval = TimeSpan.FromMilliseconds(1d);
			timer.Tick += OnTimerTick;
			timer.Start();

			void OnTimerTick(DispatcherQueueTimer sender, object args)
			{
				action();
				timer.Tick -= OnTimerTick;
			}
		}
	}
}
