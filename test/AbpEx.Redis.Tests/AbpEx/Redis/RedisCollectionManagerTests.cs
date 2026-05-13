using StackExchange.Redis;

namespace AbpEx.Redis;

public class RedisCollectionManagerTests(AbpRedisTestsFixture fixture) : AbpRedisTests(fixture)
{
    [RetryFact]
    public void GetList_Test()
    {
        if (Skip)
            return;

        var key = nameof(GetList_Test).ToLower();
        var manager = Services.GetRequiredService<IRedisCollectionManager>();
        var provider = Services.GetRequiredService<IRedisCachingProvider>();
        var col = manager.GetList<string>(key);

        var colKey = col.Key;
        if (provider.KeyExists(colKey))
            provider.KeyDel(colKey);

        Assert.Equal(RedisCollectionType.List, col.CollectionType);
        Assert.Equal(colKey, col.Key);

        col.LPush(key);
        Assert.Equal(1, col.LLen());
        Assert.True(provider.KeyExists(colKey));
        Assert.Equal(1, provider.LLen(col.Key));
    }

    [RetryFact]
    public void GetHash_Test()
    {
        if (Skip)
            return;

        var key = nameof(GetHash_Test).ToLower();
        var manager = Services.GetRequiredService<IRedisCollectionManager>();
        var provider = Services.GetRequiredService<IRedisCachingProvider>();
        var col = manager.GetHash<string>(key);

        var colKey = col.Key;
        if (provider.KeyExists(colKey))
            provider.KeyDel(colKey);

        Assert.Equal(RedisCollectionType.Hash, col.CollectionType);
        Assert.Equal(colKey, col.Key);

        col.HSet(key, key);
        Assert.Equal(1, col.HLen());

        Assert.True(provider.KeyExists(colKey));
        Assert.Equal(1, provider.HLen(col.Key));
    }

    [RetryFact]
    public void GetSet_Test()
    {
        if (Skip)
            return;

        var key = nameof(GetSet_Test).ToLower();
        var manager = Services.GetRequiredService<IRedisCollectionManager>();
        var provider = Services.GetRequiredService<IRedisCachingProvider>();
        var col = manager.GetSet<string>(key);

        var colKey = col.Key;
        if (provider.KeyExists(colKey))
            provider.KeyDel(colKey);

        Assert.Equal(RedisCollectionType.Set, col.CollectionType);
        Assert.Equal(colKey, col.Key);

        col.SAdd(key);

        Assert.Equal(1, col.SCard());
        Assert.True(provider.KeyExists(colKey));
        Assert.Equal(1, provider.SCard(col.Key));
    }

    [RetryFact]
    public void GetSortedSet_Test()
    {
        if (Skip)
            return;

        var key = nameof(GetSortedSet_Test).ToLower();
        var manager = Services.GetRequiredService<IRedisCollectionManager>();
        var provider = Services.GetRequiredService<IRedisCachingProvider>();
        var database = Services.GetRequiredService<IEasyCachingProvider>().Database.CastTo<IDatabase>();

        var col = manager.GetSortedSet<string>(key);

        var colKey = col.Key;
        if (provider.KeyExists(colKey))
            provider.KeyDel(colKey);

        Assert.Equal(RedisCollectionType.SortedSet, col.CollectionType);
        Assert.Equal(colKey, col.Key);

        col.ZAdd(key, 1);

        Assert.Equal(1, database.SortedSetLength(colKey));
        Assert.Equal(1, col.ZCount(0, 10));
        Assert.True(provider.KeyExists(colKey));
        Assert.Equal(1, provider.ZCount(colKey, 0, 10));
    }
}