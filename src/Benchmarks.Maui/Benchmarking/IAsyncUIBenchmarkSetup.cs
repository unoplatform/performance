namespace Benchmarks.Maui.Benchmarking
{
    internal interface IAsyncUIBenchmarkSetup
    {
        Task SetupAsync();
        Task TeardownAsync();
    }
}
