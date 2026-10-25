using System.Diagnostics;
using lab9v4;

Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("1. HashSet<T>: унікальні URL");
var registry = new UrlRegistry();
string[] incoming =
{
    "https://example.com/",
    "https://example.com/about",
    "https://example.com/",
    "https://example.com/contact",
    "https://example.com/ABOUT"
};

foreach (string url in incoming)
{
    bool added = registry.Add(url);
    Console.WriteLine($"   {url} - {(added ? "додано" : "дублікат, ігноруємо")}");
}

Console.WriteLine($"   Надійшло URL: {incoming.Length}, унікальних: {registry.Count}");
Console.WriteLine($"   Contains(\"https://example.com/about\"): {registry.Contains("https://example.com/about")}");
Console.WriteLine($"   Contains(\"https://example.com/blog\"): {registry.Contains("https://example.com/blog")}");

bool removed = registry.Remove("https://example.com/contact");
Console.WriteLine($"   Remove(\"https://example.com/contact\"): {removed}, залишилось: {registry.Count}");

var otherUrls = new[] { "https://example.com/about", "https://example.com/blog" };

var union = new UrlRegistry(registry.GetAll());
union.UnionWith(otherUrls);
Console.WriteLine($"   UnionWith: {string.Join(", ", union.GetAll())}");

var intersect = new UrlRegistry(registry.GetAll());
intersect.IntersectWith(otherUrls);
Console.WriteLine($"   IntersectWith: {string.Join(", ", intersect.GetAll())}");

var except = new UrlRegistry(registry.GetAll());
except.ExceptWith(otherUrls);
Console.WriteLine($"   ExceptWith: {string.Join(", ", except.GetAll())}");

Console.WriteLine("\n2. Queue<T>: черга запитів (FIFO)");
var queue = new RequestQueue();
queue.Enqueue(new PageRequest(1, "GET", "https://example.com/"));
queue.Enqueue(new PageRequest(2, "POST", "https://example.com/login"));
queue.Enqueue(new PageRequest(3, "GET", "https://example.com/about"));
queue.Enqueue(new PageRequest(4, "GET", "https://example.com/contact"));
Console.WriteLine($"   У черзі запитів: {queue.Count}");
Console.WriteLine($"   Peek (наступний на обробку): {queue.Peek()}");

while (queue.Count > 0)
{
    PageRequest request = queue.Dequeue();
    Console.WriteLine($" Обробка: {request} (залишилось: {queue.Count})");
}

Console.WriteLine("\n3. Stack<T>: історія навігації (Back)");
var history = new NavigationHistory();
history.Visit("https://example.com/");
history.Visit("https://example.com/about");
history.Visit("https://example.com/contact");
history.Visit("https://example.com/blog");
Console.WriteLine($" Поточна сторінка (Peek): {history.Current}");
Console.WriteLine($" Історія (від останньої): {string.Join(" <- ", history.GetAll())}");

Console.WriteLine($" Back  {history.Back()}");
Console.WriteLine($" Back  {history.Back()}");
Console.WriteLine($" Поточна сторінка: {history.Current}, записів в історії: {history.Count}");

Console.WriteLine("\n4. Порівняння продуктивності: List<T> та HashSet<T>");
const int size = 100000;
const int lookups = 1000;
var list = new List<string>();
var set = new HashSet<string>();
for (int i = 0; i < size; i++)
{
    string url = $"https://example.com/page{i}";
    list.Add(url);
    set.Add(url);
}

string target = $"https://example.com/page{size - 1}";

var stopwatch = Stopwatch.StartNew();
for (int i = 0; i < lookups; i++)
{
    list.Contains(target);
}
stopwatch.Stop();
Console.WriteLine($"   List<string>.Contains x{lookups}: {stopwatch.ElapsedMilliseconds} мс");

stopwatch.Restart();
for (int i = 0; i < lookups; i++)
{
    set.Contains(target);
}
stopwatch.Stop();
Console.WriteLine($"   HashSet<string>.Contains x{lookups}: {stopwatch.ElapsedMilliseconds} мс");
