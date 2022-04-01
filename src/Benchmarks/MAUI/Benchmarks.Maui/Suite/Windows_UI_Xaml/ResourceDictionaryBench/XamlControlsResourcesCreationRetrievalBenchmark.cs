#pragma warning disable 105 // Disabled until the tree is migrate to WinUI

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BenchmarkDotNet.Attributes;


namespace Benchmarks.Maui.Suite.Windows_UI_Xaml.ResourceDictionaryBench
{ 
	public class XamlControlsResourcesCreationRetrievalBenchmark
	{
		[Benchmark]
		public void Create_XamlControlsResources_And_Retrieve_Style()
		{
			var xcr = new ResourceDictionary();
			
			var style = xcr["ListViewItemExpanded"] as Style;
			var templateSetter = style.Setters.FirstOrDefault();

			if (templateSetter != null)
			{
				var template = templateSetter.Value;
			}
		}
	}
}
