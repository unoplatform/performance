using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.InProcess;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Microbenchmarks
{
	public class MicroBenchmarkRunner
	{
		public Assembly Assembly { get; }

		public MicroBenchmarkRunner(Assembly assembly)
		{
			Assembly = assembly;
		}

		public void Run()
		{
			var logger = new ConsoleLogger();

			try
			{
				var config = new CoreConfig(logger);

				var types = EnumerateBenchmarks(config).ToArray();

				foreach (var type in types)
				{
					SetStatus($"Running benchmarks for {type}");
					var b = BenchmarkRunner.Run(type, config);
				}

				SetStatus($"Done.");
			}
			catch (Exception e)
			{
				SetStatus($"Failed {e?.Message}");
				logger.WriteLine(LogKind.Error, e?.ToString());
			}
		}

		private void SetStatus(string status)
			=> Console.WriteLine($"Status: {status}");

		private IEnumerable<Type> EnumerateBenchmarks(IConfig config)
			=> from type in Assembly.GetTypes()
			   where !type.IsGenericType
			   where BenchmarkConverter.TypeToBenchmarks(type, config).BenchmarksCases.Length != 0
			   select type;

		public class CoreConfig : ManualConfig
		{
			public CoreConfig(ILogger logger)
			{
				Add(logger);
				Add(AsciiDocExporter.Default);
				Add(Job.InProcess
					.WithLaunchCount(1)
					.WithWarmupCount(1)
					.WithIterationCount(5)
					.With(InProcessToolchain.Synchronous)
					.WithId("InProcess")
				);

				ArtifactsPath = Path.GetTempPath();
			}
		}

		private class ConsoleLogger : ILogger
		{
			public ConsoleLogger() { }

			public void Flush() { }

			public void Write(LogKind logKind, string text)
			{
				Console.Write($"{logKind}: {text}");
			}

			public void WriteLine() => Console.WriteLine();

			public void WriteLine(LogKind logKind, string text)
			{
				Write(logKind, text);
				WriteLine();
			}
		}
	}
}
