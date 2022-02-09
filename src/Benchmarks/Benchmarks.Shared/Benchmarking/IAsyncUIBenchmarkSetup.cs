using System.Threading.Tasks;

namespace Benchmarks.Shared.Benchmarking
{
    internal interface IAsyncUIBenchmarkSetup
    {
        Task SetupAsync();
        Task TeardownAsync();
    }
}
