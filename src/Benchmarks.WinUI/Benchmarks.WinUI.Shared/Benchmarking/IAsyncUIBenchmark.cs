using System.Threading.Tasks;

namespace Benchmarks.WinUI.Shared.Benchmarking
{
    internal interface IAsyncUIBenchmark
    {
        Task BenchmarkAsync();
    }
}
