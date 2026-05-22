using AbpEx.Caching.Configuration;

namespace AbpEx.Redis;

[CollectionDefinition(nameof(AbpRedisTestsCollection))]
public class AbpRedisTestsCollection : ICollectionFixture<RedisTestsFixture>;

[EnableParallelization]
[Collection(nameof(AbpRedisTestsCollection))]
public class RedisTests : CoreTests<RedisTestsModule, RedisTestsFixture>
{
    private readonly Lazy<AbpRedisOptions> _abpRedisOptions;
    public AbpRedisOptions AbpRedisOptions => _abpRedisOptions.Value;

    private readonly Lazy<AbpCacheOptions> _abpCacheOptions;
    public AbpCacheOptions ReadOnlyCacheOptions => _abpCacheOptions.Value;

    protected RedisTests(RedisTestsFixture fixture) : base(fixture)
    {
        _abpRedisOptions = new Lazy<AbpRedisOptions>(() => Services.GetOptions<AbpRedisOptions>(), true);
        _abpCacheOptions = new Lazy<AbpCacheOptions>(() => Services.GetOptions<AbpCacheOptions>(), true);
    }
}