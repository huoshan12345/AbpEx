namespace AbpEx.Redis;

[CollectionDefinition(nameof(RedisUnreachableTestsCollection))]
public class RedisUnreachableTestsCollection : ICollectionFixture<RedisUnreachableTestsFixture>;

[EnableParallelization]
[Collection(nameof(RedisUnreachableTestsCollection))]
public class RedisUnreachableTests(RedisUnreachableTestsFixture fixture)
    : CoreTests<RedisTestsModule, RedisUnreachableTestsFixture>(fixture)
{
}