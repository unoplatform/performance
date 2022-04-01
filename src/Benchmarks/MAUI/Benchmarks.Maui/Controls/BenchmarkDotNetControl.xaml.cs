using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using Benchmarks.Shared.Controls;
using System.IO.Compression;
using System.Reflection;
using System.Text;

namespace Benchmarks.Maui.Controls;

public partial class BenchmarkDotNetControl : ContentPage
{
	public BenchmarkDotNetControl()
	{
		InitializeComponent();
	}

	private const string BenchmarksBaseNamespace = "SamplesApp.Benchmarks.Suite";
	private TextBlockLogger _logger;

	public string ResultsAsBase64
	{
		get => (string)GetValue(ResultsAsBase64Property);
		set => SetValue(ResultsAsBase64Property, value);
	}


	public static readonly BindableProperty ResultsAsBase64Property =
		BindableProperty.Create("ResultsAsBase64", typeof(string), typeof(BenchmarkDotNetControl), "");

	public string ClassFilter { get; set; } = "";

	private void OnRunTests(object sender, object args)
	{
		_ = Dispatcher.Dispatch(async () => await this.Run());
		
	}

	private async Task Run()
	{
		_logger = new TextBlockLogger(runLogs, true);

		try
		{
			var config = new CoreConfig(_logger);

			BenchmarkUIHost.Root = testHost;
			
			await SetStatus("Discovering benchmarks in " + BenchmarksBaseNamespace);
			var types = EnumerateBenchmarks(config).ToArray();

			int currentCount = 0;
			foreach (var type in types)
			{
				runCount.Text = (++currentCount).ToString();

				await SetStatus($"Running benchmarks for {type}");
				var b = BenchmarkRunner.Run(type, config);

				for (int i = 0; i < 3; i++)
				{
					Dispatcher.Dispatch(() =>
					{
						GC.Collect();
						GC.WaitForPendingFinalizers();
					});
				}
			}

			await SetStatus($"Finished");

			ArchiveTestResult(config);
		}
		catch (Exception e)
		{
			await SetStatus($"Failed {e?.Message}");
			_logger.WriteLine(LogKind.Error, e?.ToString());
		}
		finally
		{
			BenchmarkUIHost.Root = null;
		}
	}

	private void ArchiveTestResult(CoreConfig config)
	{
		var archiveName = BenchmarkResultArchiveName;

		if (File.Exists(archiveName))
		{
			File.Delete(archiveName);
		}

		ZipFile.CreateFromDirectory(config.ArtifactsPath, archiveName, CompressionLevel.Optimal, false);

		downloadResults.IsEnabled = true;

		ResultsAsBase64 = Convert.ToBase64String(File.ReadAllBytes(BenchmarkResultArchiveName));
	}

	private static string BenchmarkResultArchiveName

		=> null;// Path.Combine(Assembly.get(),., "benchmarks-results.zip");

	private void OnDownloadResults(object sender, EventArgs e)
	{
		//FileSavePicker savePicker = new FileSavePicker();

		//savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;

		//// Dropdown of file types the user can save the file as
		//savePicker.FileTypeChoices.Add("Zip Archive", new List<string>() { ".zip" });

		//// Default file name if the user does not type one in or select a file to replace
		//savePicker.SuggestedFileName = "benchmarks-results";

		//var file = await savePicker.PickSaveFileAsync();
		//if (file != null)
		//{
		//	CachedFileManager.DeferUpdates(file);

		//	await FileIO.WriteBytesAsync(file, File.ReadAllBytes(BenchmarkResultArchiveName));

		//	await CachedFileManager.CompleteUpdatesAsync(file);
		//}
	}

	private async Task SetStatus(string status)
	{
		runStatus.Text = status;
		await Task.Yield();
	}

	private IEnumerable<Type> EnumerateBenchmarks(IConfig config)
		=> from type in GetType().GetTypeInfo().Assembly.GetTypes()
		   where !type.IsGenericType
		   where type.Namespace?.StartsWith(BenchmarksBaseNamespace) ?? false
		   where BenchmarkConverter.TypeToBenchmarks(type, config).BenchmarksCases.Length != 0
		   where string.IsNullOrEmpty(ClassFilter)
				 || type.Name.IndexOf(ClassFilter, StringComparison.InvariantCultureIgnoreCase) >= 0
		   select type;

	public class CoreConfig : ManualConfig
	{
		public CoreConfig(ILogger logger)
		{
//			Add(logger);

//			Add(AsciiDocExporter.Default);
//			Add(JsonExporter.Full);
//			Add(CsvExporter.Default);
//			Add(BenchmarkDotNet.Exporters.Xml.XmlExporter.Full);

//			Add(Job.InProcess
//				.WithLaunchCount(1)
//				.WithWarmupCount(1)
//				.WithIterationCount(5)
//				.WithIterationTime(TimeInterval.FromMilliseconds(100))
//#if __IOS__
//					// Fails on iOS with code generation used by EmitInvokeMultiple
//					.WithUnrollFactor(1)
//#endif
//					.With(InProcessToolchain.Synchronous)
//				.WithId("InProcess")
//			);

//			ArtifactsPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "benchmarks");
		}
	}

	private class TextBlockLogger : ILogger
	{
		private static Dictionary<LogKind, Color> ColorfulScheme { get; } =
		   new Dictionary<LogKind, Color>
		   {
					{ LogKind.Default, Colors.Gray },
					{ LogKind.Help, Colors.DarkGreen },
					{ LogKind.Header, Colors.Magenta },
					{ LogKind.Result, Colors.DarkCyan },
					{ LogKind.Statistic, Colors.Cyan },
					{ LogKind.Info, Colors.DarkOrange },
					{ LogKind.Error, Colors.Red },
					{ LogKind.Hint, Colors.DarkCyan }
		   };

		private readonly Entry _target;
		private LogKind _minLogKind;

		public TextBlockLogger(Entry target, bool isDebug)
		{
			_target = target;
			_minLogKind = isDebug ? LogKind.Default : LogKind.Statistic;
		}

		public void Flush() { }

		public void Write(LogKind logKind, string text)
		{
			if (logKind >= _minLogKind)
			{
				Logger.AppendLine(text);
				_target.Text = Logger.ToString();
				_target.TextColor = GetLogKindColor(logKind);
			}
		}

		public static Color GetLogKindColor(LogKind logKind)
		{
			if (!ColorfulScheme.TryGetValue(logKind, out var brush))
			{
				brush = ColorfulScheme[LogKind.Default];
			}

			return brush;
		}

		public void WriteLine() => Logger.AppendLine(String.Empty);


		public void WriteLine(LogKind logKind, string text)
		{
			if (logKind >= _minLogKind)
			{
				Write(logKind, text);
				WriteLine();
			}
		}    

     
        private StringBuilder Logger { get; set; } = new();

        public string Id => throw new NotImplementedException();

        public int Priority => throw new NotImplementedException();
    }


}

