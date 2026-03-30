using Meziantou.Xunit.v3;

namespace AbpEx;

[CollectionDefinition(nameof(AbpExTestsCollection))]
public class AbpExTestsCollection : ICollectionFixture<AbpExTestsFixture>;

[EnableParallelization]
[Collection(nameof(AbpExTestsCollection))]
public class AbpExTests(AbpExTestsFixture fixture)
    : AbpTests<AbpExTestsFixture, AbpExTestsModule>(fixture)
{
}
