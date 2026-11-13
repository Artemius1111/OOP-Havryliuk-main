using IndependentWork13;

namespace IndependentWork13.Tests;

public class CacheTests
{
    [Fact]
    public void Add_NewItem_ItemCanBeRetrieved()
    {
        var cache = new Cache<string, int>(3);

        cache.Add("a", 1);

        Assert.Equal(1, cache.Get("a"));
    }

    [Fact]
    public void Get_ExistingKey_ReturnsValueAddedForThisKey()
    {
        var cache = new Cache<int, string>(3);
        cache.Add(1, "один");
        cache.Add(2, "два");

        string value = cache.Get(2);

        Assert.Equal("два", value);
    }

    [Fact]
    public void Get_MissingKey_ThrowsKeyNotFoundException()
    {
        var cache = new Cache<int, string>(3);
        cache.Add(1, "один");

        Assert.Throws<KeyNotFoundException>(() => cache.Get(42));
    }

    [Fact]
    public void Get_EmptyCache_ThrowsKeyNotFoundException()
    {
        var cache = new Cache<string, string>(2);

        Assert.Throws<KeyNotFoundException>(() => cache.Get("x"));
    }

    [Fact]
    public void Add_WhenCacheIsFull_RemovesOldestItem()
    {
        var cache = new Cache<int, string>(2);
        cache.Add(1, "один");
        cache.Add(2, "два");

        cache.Add(3, "три");

        Assert.False(cache.ContainsKey(1));
        Assert.True(cache.ContainsKey(2));
        Assert.True(cache.ContainsKey(3));
    }

    [Fact]
    public void Add_SeveralOverflows_RemovesItemsInInsertionOrder()
    {
        var cache = new Cache<int, string>(3);
        cache.Add(1, "один");
        cache.Add(2, "два");
        cache.Add(3, "три");

        cache.Add(4, "чотири");
        cache.Add(5, "п'ять");

        Assert.Equal(new[] { 3, 4, 5 }, cache.Keys);
    }

    [Fact]
    public void Add_WhenCacheIsFull_ReadingOldestItemDoesNotSaveItFromEviction()
    {
        var cache = new Cache<int, string>(2);
        cache.Add(1, "один");
        cache.Add(2, "два");
        cache.Get(1);

        cache.Add(3, "три");

        Assert.False(cache.ContainsKey(1));
    }

    [Fact]
    public void Add_ExistingKey_UpdatesValueWithoutEviction()
    {
        var cache = new Cache<int, string>(2);
        cache.Add(1, "один");
        cache.Add(2, "два");

        cache.Add(1, "ONE");

        Assert.Equal("ONE", cache.Get(1));
        Assert.True(cache.ContainsKey(2));
        Assert.Equal(2, cache.Count);
    }

    [Fact]
    public void ContainsKey_ReturnsTrueOnlyForStoredKeys()
    {
        var cache = new Cache<string, int>(2);
        cache.Add("a", 1);

        Assert.True(cache.ContainsKey("a"));
        Assert.False(cache.ContainsKey("b"));
    }

    [Fact]
    public void Count_NewCache_IsZero()
    {
        var cache = new Cache<int, int>(5);

        Assert.Equal(0, cache.Count);
    }

    [Fact]
    public void Count_GrowsWithEachNewKey()
    {
        var cache = new Cache<int, int>(5);

        cache.Add(1, 10);
        cache.Add(2, 20);
        cache.Add(3, 30);

        Assert.Equal(3, cache.Count);
    }

    [Fact]
    public void Count_NeverExceedsCapacity()
    {
        var cache = new Cache<int, int>(3);

        for (int i = 0; i < 10; i++)
        {
            cache.Add(i, i * i);
        }

        Assert.Equal(3, cache.Count);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Constructor_NotPositiveCapacity_ThrowsArgumentOutOfRangeException(int capacity)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Cache<int, int>(capacity));
    }
}
