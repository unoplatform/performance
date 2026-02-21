using System;
using System.Collections.Generic;
using System.Text;
using BenchmarkDotNet.Attributes;
using Microsoft.Foundation;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace SamplesApp.Benchmarks.Suite.Windows_UI_Xaml_Controls.DependencyPropertyBench
{
	public class SpecializedDPBenchmark
	{

		[GlobalSetup]
		public void Setup()
		{
		}

		[Benchmark()]
		public void Property_GetMetadataNotSelf()
		{
			Border.TagProperty.GetMetadata(typeof(Border));
		}
	}
}
