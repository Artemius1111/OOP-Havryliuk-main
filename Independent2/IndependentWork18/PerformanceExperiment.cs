using System.Diagnostics;

namespace IndependentWork18;

public record BenchmarkResult(int Size, double LinqMs, double PlinqMs, bool SameResult)
{
    public double Speedup => LinqMs / PlinqMs;
}

public static class PerformanceExperiment
{
    public static void Warmup()
    {
        List<int> data = Workloads.CreateData(10_000, seed: 1);
        RunHeavy(data);
        RunLight(data);
    }

    public static BenchmarkResult RunHeavy(List<int> data)
    {
        var linqWatch = Stopwatch.StartNew();
        long linqResult = data
            .Where(n => n % 2 == 1)
            .Select(n => (long)Workloads.CollatzSteps(n))
            .Sum();
        linqWatch.Stop();

        var plinqWatch = Stopwatch.StartNew();
        long plinqResult = data
            .AsParallel()
            .Where(n => n % 2 == 1)
            .Select(n => (long)Workloads.CollatzSteps(n))
            .Sum();
        plinqWatch.Stop();

        return new BenchmarkResult(data.Count, linqWatch.Elapsed.TotalMilliseconds, plinqWatch.Elapsed.TotalMilliseconds, linqResult == plinqResult);
    }

    public static BenchmarkResult RunLight(List<int> data)
    {
        var linqWatch = Stopwatch.StartNew();
        long linqResult = data
            .Where(n => n % 2 == 1)
            .Select(n => (long)n * 2)
            .Sum();
        linqWatch.Stop();

        var plinqWatch = Stopwatch.StartNew();
        long plinqResult = data
            .AsParallel()
            .Where(n => n % 2 == 1)
            .Select(n => (long)n * 2)
            .Sum();
        plinqWatch.Stop();

        return new BenchmarkResult(data.Count, linqWatch.Elapsed.TotalMilliseconds, plinqWatch.Elapsed.TotalMilliseconds, linqResult == plinqResult);
    }
}
