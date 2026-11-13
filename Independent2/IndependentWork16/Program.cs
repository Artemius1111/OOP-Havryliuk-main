using System.Text;

namespace IndependentWork16;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        var index = new InvertedIndex();
        foreach (Document doc in DocumentGenerator.CreateSampleDocuments())
        {
            index.AddDocument(doc);
        }

        Console.WriteLine($"Loaded {index.DocumentCount} documents");
        Console.WriteLine($"Index contains {index.UniqueWords} unique words");
        Console.WriteLine();

        Console.WriteLine("1. Пошук за індексом");
        PrintResults("SearchSingleWord(\"programming\")", index.SearchSingleWord("programming"));
        PrintResults("SearchSingleWord(\"Java\")", index.SearchSingleWord("Java"));
        PrintResults("SearchMultipleWords(\"c#\", \"async\"), AND через IntersectWith", index.SearchMultipleWords("c#", "async"));
        PrintResults("SearchMultipleWords(\"popular\", \"language\")", index.SearchMultipleWords("popular", "language"));
        PrintResults("SearchAnyWord(\"java\", \"python\"), OR через UnionWith", index.SearchAnyWord("java", "python"));
        PrintResults("SearchSingleWord(\"rust\")", index.SearchSingleWord("rust"));

        Console.WriteLine("   SearchRanked(\"c#\", \"java\", \"python\", \"popular\"), сортування за кількістю збігів:");
        foreach ((Document doc, int matches) in index.SearchRanked("c#", "java", "python", "popular"))
        {
            Console.WriteLine($"      {doc}, збігів: {matches}");
        }
        Console.WriteLine();

        Console.WriteLine("2. Вимірювання продуктивності: пошук слова \"linq\"");
        Console.WriteLine("   Лінійний пошук повторено 20 разів, пошук через індекс 1000 разів, у таблиці середній час одного пошуку");
        PerformanceTest.Warmup();

        foreach (int count in new[] { 10, 100, 1000, 10000 })
        {
            PerformanceResult result = PerformanceTest.CompareSearchMethods(DocumentGenerator.Generate(count), "linq");
            PrintPerformance(result);
        }
        Console.WriteLine();

        Console.WriteLine("3. Інтерактивний пошук (AND)");
        RunInteractiveSearch(index);
    }

    static void PrintResults(string title, List<Document> results)
    {
        string found = results.Count == 0 ? "нічого не знайдено" : string.Join(", ", results);
        Console.WriteLine($"   {title}: {found}");
    }

    static void PrintPerformance(PerformanceResult result)
    {
        double speedup = result.LinearMicroseconds / result.IndexMicroseconds;
        double savedPerSearch = result.LinearMicroseconds - result.IndexMicroseconds;
        string payback = savedPerSearch > 0
            ? $"індекс окупається після {Math.Ceiling(result.BuildMilliseconds * 1000 / savedPerSearch)} пошуків"
            : "індекс не окупається";

        Console.WriteLine($"   {result.DocumentCount} документів: лінійний {result.LinearMicroseconds:F2} мкс, індекс {result.IndexMicroseconds:F2} мкс, швидше у {speedup:F1} раза");
        Console.WriteLine($"      знайдено: лінійний {result.LinearFound}, індекс {result.IndexFound}; побудова індексу {result.BuildMilliseconds:F3} мс, {payback}");
    }

    static void RunInteractiveSearch(InvertedIndex index)
    {
        while (true)
        {
            Console.Write("Enter search query (or 'exit'): ");
            string? query = Console.ReadLine();

            if (query is null || query.Trim().Equals("exit", StringComparison.OrdinalIgnoreCase))
            {
                break;
            }

            string[] words = TextProcessor.Tokenize(query).ToArray();

            if (words.Length == 0)
            {
                continue;
            }

            Console.WriteLine();
            Console.WriteLine("Search Results (AND):");
            List<Document> results = index.SearchMultipleWords(words);

            foreach (Document doc in results)
            {
                Console.WriteLine(doc);
                Console.WriteLine($"  {doc.Content.Substring(0, Math.Min(100, doc.Content.Length))}...");
            }

            Console.WriteLine($"\nFound {results.Count} documents\n");
        }
    }
}
