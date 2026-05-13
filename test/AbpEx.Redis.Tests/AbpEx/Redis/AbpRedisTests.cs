using AbpEx.Caching.Configuration;
using Volo.Abp;

namespace AbpEx.Redis;

[CollectionDefinition(nameof(AbpRedisTestsCollection))]
public class AbpRedisTestsCollection : ICollectionFixture<AbpRedisTestsFixture>;

[EnableParallelization]
[Collection(nameof(AbpRedisTestsCollection))]
public class AbpRedisTests : AbpTests<AbpRedisTestsFixture, AbpRedisTestsModule>
{
    private readonly Lazy<AbpRedisOptions> _abpRedisOptions;
    public AbpRedisOptions AbpRedisOptions => _abpRedisOptions.Value;

    private readonly Lazy<AbpCacheOptions> _abpCacheOptions;
    public AbpCacheOptions ReadOnlyCacheOptions => _abpCacheOptions.Value;

    protected AbpRedisTests(AbpRedisTestsFixture fixture) : base(fixture)
    {
        _abpRedisOptions = new Lazy<AbpRedisOptions>(() => Services.GetOptions<AbpRedisOptions>(), true);
        _abpCacheOptions = new Lazy<AbpCacheOptions>(() => Services.GetOptions<AbpCacheOptions>(), true);
    }

    // there is no Redis server in GitHub Action Windows runner.
    public static bool Skip => TestHelper.IsGithubAction && TestHelper.IsWindows;
}