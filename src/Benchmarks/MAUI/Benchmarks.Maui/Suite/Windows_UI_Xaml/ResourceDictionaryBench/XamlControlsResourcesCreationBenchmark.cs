using BenchmarkDotNet.Attributes;

namespace Benchmarks.Maui.Suite.Windows_UI_Xaml.ResourceDictionaryBench
{
	public class XamlControlsResourcesCreationBenchmark
	{
		[Benchmark]
		public void Create_XamlControlsResources()
		{
			var xcr = new ResourceDictionary();
		}
	}
}
