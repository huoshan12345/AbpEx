using Volo.Abp.Modularity;

namespace AbpEx;

public class CoreTests<TModule, TFixture>(TFixture fixture)
    where TFixture : CoreTestsFixture<TModule>
    where TModule : AbpModule
{
    protected TFixture Fixture => fixture;
    protected IServiceProvider Services => Fixture.Services;
    protected ITestOutputHelper? Output => Fixture.Output;
    protected CancellationToken CancellationToken => Fixture.CancellationToken;
}

[CollectionDefinition(nameof(CoreTestsCollection))]
public class CoreTestsCollection : ICollectionFixture<CoreTestsFixture>;

[EnableParallelization]
[Collection(nameof(CoreTestsCollection))]
public class CoreTests(CoreTestsFixture fixture)
    : CoreTests<CoreTestsTestsModule, CoreTestsFixture>(fixture)
{
    [ModuleInitializer]
    public static void Initialize()
    {
        ThreadPool.SetMinThreads(100, 100);
#pragma warning disable SYSLIB0014
        ServicePointManager.DefaultConnectionLimit = short.MaxValue;
#pragma warning restore SYSLIB0014
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }
}