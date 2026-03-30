namespace AbpEx.Redis;

public class RedisHashTests(AbpRedisTestsFixture fixture) : AbpRedisTests(fixture)
{
    [RetryFact]
    public void HSet_HGet_String_Test()
    {
        var key = nameof(HSet_HGet_String_Test).ToLower();
        var manager = Services.GetRequiredService<IRedisCollectionManager>();
        var col = manager.GetHash<string>(key);

        col.HSet("1", "11");
        var actual = col.HGet("1");
        Assert.Equal("11", actual);
    }


    [RetryFact]
    public void Provider_HmSet_HmGet_String_Test()
    {
        var key = nameof(Provider_HmSet_HmGet_String_Test).ToLower();
        var manager = Services.GetRequiredService<IRedisCollectionManager>();
        var col = manager.GetHash<string>(key);
        var colKey = col.Key;
        var provider = Services.GetRequiredService<IRedisCachingProvider>();
        var dic = Enumerable.Range(1, 10)
            .Select(m => m.ToString())
            .ToDictionary(m => m, m => m + m);

        provider.HMSet(colKey, dic, TimeSpan.FromMinutes(1));
        var actual = provider.HMGet(colKey, dic.Keys.ToList());
        Assert.Equal(dic, actual);
    }

    [RetryFact]
    public void HmSet_HmGet_String_Test()
    {
        var key = nameof(HmSet_HmGet_String_Test).ToLower();
        var manager = Services.GetRequiredService<IRedisCollectionManager>();
        var col = manager.GetHash<string>(key);

        var dic = Enumerable.Range(1, 10)
            .Select(m => m.ToString())
            .ToDictionary(m => m, m => m + m);

        col.HmSet(dic, TimeSpan.FromMinutes(1));
        var actual = col.HmGet(dic.Keys.ToList());
        Assert.Equal(dic, actual);
    }


    [RetryFact]
    public async Task HmSetAsync_HmGetAsync_String_Test()
    {
        var key = nameof(HmSetAsync_HmGetAsync_String_Test).ToLower();
        var manager = Services.GetRequiredService<IRedisCollectionManager>();
        var col = manager.GetHash<string>(key);

        var dic = Enumerable.Range(1, 10)
            .Select(m => m.ToString())
            .ToDictionary(m => m, m => m + m);

        await col.HmSetAsync(dic, TimeSpan.FromMinutes(1));
        var actual = await col.HmGetAsync(dic.Keys.ToList());
        Assert.Equal(dic, actual);
    }
}