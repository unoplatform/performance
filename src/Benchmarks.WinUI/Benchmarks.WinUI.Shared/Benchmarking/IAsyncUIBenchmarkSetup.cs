using System.Threading.Tasks;

namespace Benchmarks.WinUI.Shared.Benchmarking
{
    internal interface IAsyncUIBenchmarkSetup
    {
        Task SetupAsync();
        Task TeardownAsync();
    }
}
