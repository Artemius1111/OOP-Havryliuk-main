namespace lab9v4;

public class UrlRegistry
{
    private readonly HashSet<string> _urls;

    public UrlRegistry()
    {
        _urls = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    }

    public UrlRegistry(IEnumerable<string> urls)
    {
        _urls = new HashSet<string>(urls, StringComparer.OrdinalIgnoreCase);
    }

    public int Count => _urls.Count;

    public bool Add(string url) => _urls.Add(url);

    public bool Remove(string url) => _urls.Remove(url);

    public bool Contains(string url) => _urls.Contains(url);

    public void UnionWith(IEnumerable<string> other) => _urls.UnionWith(other);

    public void IntersectWith(IEnumerable<string> other) => _urls.IntersectWith(other);

    public void ExceptWith(IEnumerable<string> other) => _urls.ExceptWith(other);

    public IEnumerable<string> GetAll() => _urls.OrderBy(u => u, StringComparer.Ordinal);
}
