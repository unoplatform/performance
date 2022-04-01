using System.Threading.Tasks;

namespace Benchmarks.Maui.Benchmarking
{
    internal interface IAsyncUIBenchmark
    {
        Task BenchmarkAsync();
    }
}
