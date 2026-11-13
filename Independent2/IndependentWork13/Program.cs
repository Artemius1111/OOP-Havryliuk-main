using System.Text;

namespace IndependentWork13;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        Console.WriteLine("1. Cache<int, Book> з ємністю 3 (політика FIFO)");
        DemonstrateFifo();
        Console.WriteLine();

        Console.WriteLine("2. LruCache<int, Book> з ємністю 3 (розширення LRU)");
        DemonstrateLru();
    }

    static void DemonstrateFifo()
    {
        var cache = new Cache<int, Book>(3);

        cache.Add(1, new Book("Кобзар", "Тарас Шевченко"));
        cache.Add(2, new Book("Тіні забутих предків", "Михайло Коцюбинський"));
        cache.Add(3, new Book("Лісова пісня", "Леся Українка"));
        Console.WriteLine($"   Додано 3 книги, Count = {cache.Count}, порядок ключів: {string.Join(", ", cache.Keys)}");

        Console.WriteLine($"   Get(1): {cache.Get(1)}");

        cache.Add(4, new Book("Захар Беркут", "Іван Франко"));
        Console.WriteLine($"   Add(4): кеш повний, видалено найстаріший ключ 1, порядок ключів: {string.Join(", ", cache.Keys)}");
        Console.WriteLine($"   ContainsKey(1) = {cache.ContainsKey(1)}, ContainsKey(4) = {cache.ContainsKey(4)}");

        cache.Add(5, new Book("Intermezzo", "Михайло Коцюбинський"));
        Console.WriteLine($"   Add(5): видалено ключ 2, порядок ключів: {string.Join(", ", cache.Keys)}");

        cache.Add(3, new Book("Лісова пісня (нове видання)", "Леся Українка"));
        Console.WriteLine($"   Add(3) для наявного ключа: значення оновлено без видалення, Count = {cache.Count}, Get(3): {cache.Get(3)}");

        try
        {
            cache.Get(1);
        }
        catch (KeyNotFoundException ex)
        {
            Console.WriteLine($"   Get(1): KeyNotFoundException, {ex.Message}");
        }
    }

    static void DemonstrateLru()
    {
        var cache = new LruCache<int, Book>(3);

        cache.Add(1, new Book("Кобзар", "Тарас Шевченко"));
        cache.Add(2, new Book("Тіні забутих предків", "Михайло Коцюбинський"));
        cache.Add(3, new Book("Лісова пісня", "Леся Українка"));
        Console.WriteLine($"   Додано 3 книги, порядок від давно використаних до нещодавніх: {string.Join(", ", cache.Keys)}");

        Console.WriteLine($"   Get(1): {cache.Get(1)}, порядок: {string.Join(", ", cache.Keys)}");

        cache.Add(4, new Book("Захар Беркут", "Іван Франко"));
        Console.WriteLine($"   Add(4): видалено ключ 2, бо його найдовше не використовували, порядок: {string.Join(", ", cache.Keys)}");
        Console.WriteLine($"   ContainsKey(1) = {cache.ContainsKey(1)}, ContainsKey(2) = {cache.ContainsKey(2)}");
    }
}
