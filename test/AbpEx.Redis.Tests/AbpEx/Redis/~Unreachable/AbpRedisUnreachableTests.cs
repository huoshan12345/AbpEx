namespace AbpEx.Redis;

[CollectionDefinition(nameof(AbpRedisUnreachableTestsCollection))]
public class AbpRedisUnreachableTestsCollection : ICollectionFixture<AbpRedisUnreachableTestsFixture>;

[EnableParallelization]
[Collection(nameof(AbpRedisUnreachableTestsCollection))]
public class AbpRedisUnreachableTests(AbpRedisUnreachableTestsFixture fixture)
    : AbpTests<AbpRedisUnreachableTestsFixture, AbpRedisTestsModule>(fixture)
{
}