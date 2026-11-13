namespace IndependentWork16;

public static class DocumentGenerator
{
    private static readonly string[] Vocabulary =
    {
        "cloud", "database", "server", "testing", "network", "security", "framework", "compiler",
        "algorithm", "interface", "library", "runtime", "memory", "thread", "collection", "generic",
        "delegate", "query", "index", "performance", "debugging", "refactoring", "pattern", "module"
    };

    public static List<Document> CreateSampleDocuments()
    {
        return new List<Document>
        {
            new Document
            {
                Id = 1,
                Title = "Introduction to C#",
                Content = "C# is a modern object-oriented programming language developed by Microsoft."
            },
            new Document
            {
                Id = 2,
                Title = "Java Basics",
                Content = "Java is a popular programming language known for its portability and performance."
            },
            new Document
            {
                Id = 3,
                Title = "Python Overview",
                Content = "Python is a high-level programming language with simple syntax."
            },
            new Document
            {
                Id = 4,
                Title = "C# Advanced Features",
                Content = "C# supports LINQ, async/await, and many modern programming paradigms."
            },
            new Document
            {
                Id = 5,
                Title = "Comparing Languages",
                Content = "C#, Java, and Python are among the most popular programming languages today."
            }
        };
    }

    public static List<Document> Generate(int count, int seed = 42)
    {
        var random = new Random(seed);
        List<Document> samples = CreateSampleDocuments();
        var documents = new List<Document>(count);

        for (int i = 0; i < count; i++)
        {
            Document sample = samples[i % samples.Count];
            string extraWords = string.Join(" ", Enumerable.Range(0, 8).Select(_ => Vocabulary[random.Next(Vocabulary.Length)]));

            documents.Add(new Document
            {
                Id = i + 1,
                Title = $"{sample.Title} #{i + 1}",
                Content = $"{sample.Content} {extraWords}"
            });
        }

        return documents;
    }
}
