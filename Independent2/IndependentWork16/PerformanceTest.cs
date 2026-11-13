using System.Diagnostics;

namespace IndependentWork16;

public record PerformanceResult(
    int DocumentCount,
    int LinearFound,
    int IndexFound,
    double LinearMicroseconds,
    double IndexMicroseconds,
    double BuildMilliseconds);

public static class PerformanceTest
{
    private const int LinearRuns = 20;
    private const int IndexRuns = 1000;

    public static void Warmup()
    {
        CompareSearchMethods(DocumentGenerator.CreateSampleDocuments(), "programming");
    }

    public static PerformanceResult CompareSearchMethods(List<Document> documents, string searchWord)
    {
        string normalizedWord = TextProcessor.Normalize(searchWord).First();

        var buildWatch = Stopwatch.StartNew();
        var index = new InvertedIndex();
        foreach (Document doc in documents)
        {
            index.AddDocument(doc);
        }
        buildWatch.Stop();

        List<Document> linearResults = LinearSearch(documents, normalizedWord);
        List<Document> indexResults = index.SearchSingleWord(searchWord);

        var linearWatch = Stopwatch.StartNew();
        for (int i = 0; i < LinearRuns; i++)
        {
            linearResults = LinearSearch(documents, normalizedWord);
        }
        linearWatch.Stop();

        var indexWatch = Stopwatch.StartNew();
        for (int i = 0; i < IndexRuns; i++)
        {
            indexResults = index.SearchSingleWord(searchWord);
        }
        indexWatch.Stop();

        return new PerformanceResult(
            documents.Count,
            linearResults.Count,
            indexResults.Count,
            linearWatch.Elapsed.TotalMicroseconds / LinearRuns,
            indexWatch.Elapsed.TotalMicroseconds / IndexRuns,
            buildWatch.Elapsed.TotalMilliseconds);
    }

    private static List<Document> LinearSearch(List<Document> documents, string normalizedWord)
    {
        return documents
            .Where(doc => TextProcessor.Normalize(doc.Content).Contains(normalizedWord))
            .ToList();
    }
}
