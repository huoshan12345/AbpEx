namespace AbpEx.Redis;

public class RedisCacheTests(AbpRedisTestsFixture fixture) : AbpRedisTests(fixture)
{
    public record Model(int Id, string? Name, int Age, int? CoinCount = null);

    private void Test<T>(string key, T value)
    {
        var provider = Services.GetRequiredService<IEasyCachingProvider>();
        key = key.ToLower();
        provider.Remove(key);

        var obj = provider.Get(key, () => value, TimeSpan.FromMinutes(1));
        Assert.True(obj.HasValue);
        Assert.Equal(value, obj.Value);

        var objNew = provider.Get<T>(key);
        Assert.True(objNew.HasValue);
        Assert.Equal(value, objNew.Value);
    }

    [RetryFact]
    public void Basic_Test()
    {
        var name = nameof(Basic_Test) + Environment.Version.Major;
        var provider = Services.GetRequiredService<IEasyCachingProvider>();
        Assert.IsType<PatchedRedisCachingProvider>(provider);
        var cacheManager = Services.GetRequiredService<ICacheManager>();
        var cache = cacheManager.GetCache<string>(name);
        var obj = cache.Get(name, k => name);
        Assert.True(obj.HasValue);
        var objNew = cache.Get(name);
        Assert.True(objNew.HasValue);
        Assert.Equal(obj.Value, objNew.Value);

        cache.Remove(name);
        var objRemoved = cache.Get(name);
        Assert.False(objRemoved.HasValue);
    }

    [Fact]
    public void Serializer_Test()
    {
        var provider = Services.GetRequiredService<IEasyCachingProvider>();
        Assert.IsType<PatchedRedisCachingProvider>(provider);

        var serializer = Services.GetRequiredService<IEasyCachingSerializer>();
        Assert.IsType<PatchedJsonSerializer>(serializer);

        var array = Enumerable.Range(1, 3)
            .Select((m, i) => new Model(m, m.ToString("D8"), m))
            .ToArray();

        var sep = ReadOnlyCacheOptions.Separator;
        foreach (var value in array)
        {
            Test(nameof(Model) + sep + value.Name, value); // class
            Test(nameof(Model.Name) + sep + value.Name, value.Name); // string
            Test(nameof(Model.Age) + sep + value.Age, value.Age); // int
        }
    }

    [RetryFact]
    public void GetAll_Test()
    {
        var cacheManager = Services.GetRequiredService<ICacheManager>();
        var cache = cacheManager.GetCache<string>(nameof(GetAll_Test) + Environment.Version.Major);
        var keys = Enumerable.Range(1, 3).Select(m => m.ToString()).ToArray();
        cache.RemoveAll(keys);
        foreach (var key in keys)
        {
            Assert.False(cache.Exists(key));
            cache.Set(key, key + key, TimeSpan.FromHours(1));
        }
        var all = cache.GetAll(keys);

        foreach (var key in keys)
        {
            Assert.True(all.TryGetValue(key, out var value));
            Assert.True(value.HasValue);
            Assert.Equal(key + key, value.Value);
        }
    }

    [RetryFact]
    public async Task GetAllAsync_Test()
    {
        var cacheManager = Services.GetRequiredService<ICacheManager>();
        var cache = cacheManager.GetCache<string>(nameof(GetAllAsync_Test) + Environment.Version.Major);
        var keys = Enumerable.Range(1, 3).Select(m => m.ToString()).ToArray();
        await cache.RemoveAllAsync(keys);
        foreach (var key in keys)
        {
            var exist = await cache.ExistsAsync(key);
            Assert.False(exist, key);
            await cache.SetAsync(key, key + key, TimeSpan.FromHours(1));
        }
        var all = await cache.GetAllAsync(keys);

        foreach (var key in keys)
        {
            AssertEx.True(all.TryGetValue(key, out var value), () => $"keys: {all.Keys.JoinWith(" ")}, cache key: {cache.Prefix}");
            Assert.True(value!.HasValue);
            Assert.Equal(key + key, value.Value);
        }
    }
}