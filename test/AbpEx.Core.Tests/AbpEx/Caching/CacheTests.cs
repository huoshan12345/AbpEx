namespace AbpEx.Caching;

public class CacheTests(CoreTestsFixture fixture) : CoreTests(fixture)
{
    [Fact]
    public void TestCache()
    {
        const string str = "test";
        var cacheManager = Services.GetRequiredService<ICacheManager>();
        var cache = cacheManager.GetCache<string>(str);
        var obj = cache.Get(str, k => str);
        Assert.True(obj.HasValue);
        var objNew = cache.Get(str);
        Assert.True(objNew.HasValue);
        Assert.Equal(obj.Value, objNew.Value);

        cache.Remove(str);
        var objRemoved = cache.Get(str);
        Assert.False(objRemoved.HasValue);
    }
}