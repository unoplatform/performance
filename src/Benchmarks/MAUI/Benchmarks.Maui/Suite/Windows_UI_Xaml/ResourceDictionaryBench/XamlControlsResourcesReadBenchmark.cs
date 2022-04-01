#pragma warning disable 105 // Disabled until the tree is migrate to WinUI

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BenchmarkDotNet.Attributes;

namespace Benchmarks.Maui.Suite.Windows_UI_Xaml.ResourceDictionaryBench
{
	public class XamlControlsResourcesReadBenchmark
	{
		ResourceDictionary SUT;

		[GlobalSetup]
		public void Setup()
		{
			SUT = new ResourceDictionary();
			if (!(SUT["ListViewItemExpanded"] is Style)) {
				throw new InvalidOperationException($"ListViewItemExpanded does not exist");
			}

		}

		[Benchmark]
		public void ReadInvalidKey()
		{
			SUT.TryGetValue("InvalidKey", out var style);
		}

		[Benchmark]
		public void ReadExistingKey()
		{
			SUT.TryGetValue("ListViewItemExpanded", out var style);
		}

		[Benchmark]
		public void ReadExistingType()
		{
			SUT.TryGetValue(typeof(Frame).Name, out var style);
		}
	}
}
