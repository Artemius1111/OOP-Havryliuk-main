using IndependentWork13;

namespace IndependentWork13.Tests;

public class LruCacheTests
{
    [Fact]
    public void Add_WhenCacheIsFull_RemovesLeastRecentlyUsedItem()
    {
        var cache = new LruCache<int, string>(2);
        cache.Add(1, "один");
        cache.Add(2, "два");

        cache.Add(3, "три");

        Assert.False(cache.ContainsKey(1));
        Assert.Equal(2, cache.Count);
    }

    [Fact]
    public void Get_MovesKeyToEnd_SoAnotherKeyIsEvicted()
    {
        var cache = new LruCache<int, string>(2);
        cache.Add(1, "один");
        cache.Add(2, "два");
        cache.Get(1);

        cache.Add(3, "три");

        Assert.True(cache.ContainsKey(1));
        Assert.False(cache.ContainsKey(2));
        Assert.Equal(new[] { 1, 3 }, cache.Keys);
    }

    [Fact]
    public void Add_ExistingKey_UpdatesValueAndMarksItAsUsed()
    {
        var cache = new LruCache<int, string>(2);
        cache.Add(1, "один");
        cache.Add(2, "два");
        cache.Add(1, "ONE");

        cache.Add(3, "три");

        Assert.Equal("ONE", cache.Get(1));
        Assert.False(cache.ContainsKey(2));
    }

    [Fact]
    public void Get_MissingKey_ThrowsKeyNotFoundException()
    {
        var cache = new LruCache<string, int>(2);

        Assert.Throws<KeyNotFoundException>(() => cache.Get("x"));
    }
}
