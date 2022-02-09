using System.Threading.Tasks;

namespace Benchmarks.Shared.Benchmarking
{
    internal interface IAsyncUIBenchmark
    {
        Task BenchmarkAsync();
    }
}
