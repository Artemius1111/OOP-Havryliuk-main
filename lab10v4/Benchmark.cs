using System.Diagnostics;

namespace lab10v4;

public record Measurement(string Label, double FirstRunMs, double AverageUs);

public static class Benchmark
{
    public const int Iterations = 10_000;

    public static List<Measurement> Results { get; } = new();

    public static T Run<T>(string label, Func<T> query)
    {
        var stopwatch = Stopwatch.StartNew();
        T result = query();
        stopwatch.Stop();
        double firstRunMs = stopwatch.Elapsed.TotalMilliseconds;

        stopwatch.Restart();
        for (int i = 0; i < Iterations; i++)
        {
            query();
        }
        stopwatch.Stop();
        double averageUs = stopwatch.Elapsed.TotalMilliseconds * 1000 / Iterations;

        Results.Add(new Measurement(label, firstRunMs, averageUs));
        Console.WriteLine($"   {label}: перший запуск {firstRunMs:F4} мс, середнє з {Iterations} запусків {averageUs:F3} мкс");

        return result;
    }

    public static void PrintSummary()
    {
        Console.WriteLine("Час виконання запитів (від найповільнішого):");

        foreach (var m in Results.OrderByDescending(r => r.AverageUs))
        {
            Console.WriteLine($"   {m.Label}: середнє {m.AverageUs:F3} мкс, перший запуск {m.FirstRunMs:F4} мс");
        }
    }
}
