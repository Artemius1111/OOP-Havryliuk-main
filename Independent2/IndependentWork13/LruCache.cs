namespace IndependentWork13;

public class LruCache<TKey, TValue> where TKey : notnull
{
    private readonly int _capacity;
    private readonly Dictionary<TKey, TValue> _cache;
    private readonly LinkedList<TKey> _keysOrder;

    public LruCache(int capacity)
    {
        if (capacity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity), "Ємність кешу має бути більшою за нуль");
        }

        _capacity = capacity;
        _cache = new Dictionary<TKey, TValue>(capacity);
        _keysOrder = new LinkedList<TKey>();
    }

    public int Count => _cache.Count;

    public IReadOnlyCollection<TKey> Keys => _keysOrder.ToArray();

    public void Add(TKey key, TValue value)
    {
        if (_cache.ContainsKey(key))
        {
            _cache[key] = value;
            MarkAsUsed(key);
            return;
        }

        if (_cache.Count == _capacity)
        {
            TKey leastUsedKey = _keysOrder.First!.Value;
            _keysOrder.RemoveFirst();
            _cache.Remove(leastUsedKey);
        }

        _cache.Add(key, value);
        _keysOrder.AddLast(key);
    }

    public TValue Get(TKey key)
    {
        if (_cache.TryGetValue(key, out TValue? value))
        {
            MarkAsUsed(key);
            return value;
        }

        throw new KeyNotFoundException($"Ключ {key} відсутній у кеші");
    }

    public bool ContainsKey(TKey key)
    {
        return _cache.ContainsKey(key);
    }

    private void MarkAsUsed(TKey key)
    {
        _keysOrder.Remove(key);
        _keysOrder.AddLast(key);
    }
}
