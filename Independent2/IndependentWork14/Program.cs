using System.Text;

namespace IndependentWork14;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        Console.WriteLine("1. FileMetadataCache: кеш метаданих файлів на Dictionary<string, FileMetadata>");
        DemonstrateFileMetadataCache();
        Console.WriteLine();

        Console.WriteLine("2. TagManager: теги статей на Dictionary<string, HashSet<string>>");
        DemonstrateTagManager();
        Console.WriteLine();

        Console.WriteLine("3. SearchIndex: інвертований індекс на Dictionary<string, HashSet<int>>");
        DemonstrateSearchIndex();
        Console.WriteLine();

        Console.WriteLine("4. HashSet<Author> з власним IEqualityComparer");
        DemonstrateCustomComparer();
    }

    static void DemonstrateFileMetadataCache()
    {
        var cache = new FileMetadataCache();
        var date = new DateTime(2026, 11, 10, 14, 30, 0);

        cache.Add(FileMetadata.FromContent("README.md", "# Самостійна робота №14", date));
        cache.Add(FileMetadata.FromContent("Program.cs", "Console.WriteLine(\"Hello\");", date.AddHours(1)));
        cache.Add(FileMetadata.FromContent("TagManager.cs", "public class TagManager { }", date.AddHours(2)));
        Console.WriteLine($"   Додано файлів: {cache.Count}");

        bool added = cache.Add(FileMetadata.FromContent("readme.md", "інший вміст", date));
        Console.WriteLine($"   Add(\"readme.md\") повернув {added}: ключі порівнюються без урахування регістру, як імена файлів у Windows");

        Console.WriteLine($"   Get(\"PROGRAM.CS\"): {cache.Get("PROGRAM.CS")}");
        Console.WriteLine($"   Contains(\"TagManager.cs\") = {cache.Contains("TagManager.cs")}, Contains(\"Cache.cs\") = {cache.Contains("Cache.cs")}");

        cache.AddOrUpdate(FileMetadata.FromContent("Program.cs", "Console.WriteLine(\"Привіт, світе!\");", date.AddDays(1)));
        Console.WriteLine($"   Після AddOrUpdate: {cache.Get("Program.cs")}");

        Console.WriteLine($"   Remove(\"TagManager.cs\") = {cache.Remove("TagManager.cs")}, Remove(\"Cache.cs\") = {cache.Remove("Cache.cs")}, залишилось файлів: {cache.Count}");
        Console.WriteLine($"   Get(\"TagManager.cs\") повертає null: {cache.Get("TagManager.cs") is null}");
    }

    static void DemonstrateTagManager()
    {
        var manager = new TagManager();

        manager.AddTag("article-1", "CSharp");
        manager.AddTag("article-1", "ООП");
        manager.AddTag("article-1", "колекції");
        manager.AddTag("article-1", "csharp");

        manager.AddTag("article-2", "csharp");
        manager.AddTag("article-2", "LINQ");
        manager.AddTag("article-2", "колекції");

        manager.AddTag("article-3", "Python");
        manager.AddTag("article-3", "ООП");

        Console.WriteLine($"   article-1: {Format(manager.GetTags("article-1"))} (тег csharp не продублювався)");
        Console.WriteLine($"   article-2: {Format(manager.GetTags("article-2"))}");
        Console.WriteLine($"   article-3: {Format(manager.GetTags("article-3"))}");
        Console.WriteLine($"   Усі унікальні теги (UnionWith): {Format(manager.GetAllUniqueTags())}");
        Console.WriteLine($"   Спільні теги article-1 і article-2 (IntersectWith): {Format(manager.FindCommonTags("article-1", "article-2"))}");
        Console.WriteLine($"   Теги лише article-1, яких немає в article-2 (ExceptWith): {Format(manager.FindTagsOnlyIn("article-1", "article-2"))}");
        Console.WriteLine($"   Статті з тегом \"ооп\": {string.Join(", ", manager.FindArticlesByTag("ооп"))}");

        manager.RemoveTag("article-3", "Python");
        manager.RemoveTag("article-3", "ООП");
        Console.WriteLine($"   Після видалення всіх тегів article-3 унікальних тегів: {manager.GetAllUniqueTags().Count}");
    }

    static void DemonstrateSearchIndex()
    {
        var index = new SearchIndex();
        index.AddDocument(1, "Словник зберігає пари ключ значення і шукає за ключем");
        index.AddDocument(2, "Множина зберігає лише унікальні значення");
        index.AddDocument(3, "Словник і множина побудовані на хеш-таблиці");
        index.AddDocument(4, "Список зберігає елементи у порядку додавання");

        Console.WriteLine($"   Проіндексовано 4 документи, різних слів: {index.WordCount}");
        Console.WriteLine($"   Search(\"зберігає\"): документи {Format(index.Search("зберігає"))}");
        Console.WriteLine($"   Search(\"Словник\"): документи {Format(index.Search("Словник"))}");
        Console.WriteLine($"   SearchMultiple(\"словник\", \"множина\"): документи {Format(index.SearchMultiple("словник", "множина"))}");
        Console.WriteLine($"   SearchMultiple(\"зберігає\", \"значення\"): документи {Format(index.SearchMultiple("зберігає", "значення"))}");
        Console.WriteLine($"   Search(\"масив\"): знайдено {index.Search("масив").Count} документів");
    }

    static void DemonstrateCustomComparer()
    {
        var authors = new HashSet<Author>(new AuthorNameComparer());

        bool first = authors.Add(new Author { Name = "Олена Коваль", Email = "olena@example.com" });
        bool second = authors.Add(new Author { Name = "олена коваль", Email = "o.koval@example.com" });
        bool third = authors.Add(new Author { Name = "Іван Петренко", Email = "ivan@example.com" });

        Console.WriteLine($"   Add повернув: {first}, {second}, {third}; у множині {authors.Count} автори");
        Console.WriteLine($"   Автори: {string.Join("; ", authors)}");
    }

    static string Format<T>(IEnumerable<T> items)
    {
        return string.Join(", ", items.OrderBy(item => item));
    }
}
