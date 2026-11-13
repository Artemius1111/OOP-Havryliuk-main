using System.Text;

namespace IndependentWork18;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        Console.WriteLine($"Логічних ядер процесора: {Environment.ProcessorCount}");
        Console.WriteLine("Дані: випадкові цілі числа від 1 до 1 000 000 (Random з seed 18)");
        Console.WriteLine();

        PerformanceExperiment.Warmup();

        var heavyResults = new List<BenchmarkResult>();
        var lightResults = new List<BenchmarkResult>();

        foreach (int size in new[] { 1_000_000, 5_000_000, 10_000_000 })
        {
            List<int> data = Workloads.CreateData(size);
            heavyResults.Add(PerformanceExperiment.RunHeavy(data));
            lightResults.Add(PerformanceExperiment.RunLight(data));
        }

        Console.WriteLine("1. Важка операція: Where(непарні).Select(кількість кроків гіпотези Коллатца).Sum()");
        PrintResults(heavyResults);

        Console.WriteLine("2. Легка операція: Where(непарні).Select(n * 2).Sum()");
        PrintResults(lightResults);

        Console.WriteLine("3. Побічні ефекти в PLINQ (1 000 000 елементів)");
        SideEffectsDemo.Run(Workloads.CreateData(1_000_000));
    }

    static void PrintResults(List<BenchmarkResult> results)
    {
        foreach (BenchmarkResult result in results)
        {
            string verdict = result.Speedup >= 1
                ? $"PLINQ швидший у {result.Speedup:F2} раза"
                : $"PLINQ повільніший у {1 / result.Speedup:F2} раза";

            Console.WriteLine($"   {result.Size:N0} елементів: LINQ {result.LinqMs:F1} мс, PLINQ {result.PlinqMs:F1} мс, {verdict}, результати однакові: {result.SameResult}");
        }

        Console.WriteLine();
    }
}
