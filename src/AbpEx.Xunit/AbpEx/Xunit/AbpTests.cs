namespace AbpEx.Xunit;

public class AbpTests<TFixture, TModule>(TFixture fixture)
    where TFixture : AbpTestsFixture<TModule>
    where TModule : AbpModule
{
    protected TFixture Fixture { get; } = fixture;
    protected IServiceProvider Services => Fixture.Services;
    protected ILogger Logger => Services.CreateLogger(GetType());
#if ABPEX_XUNIT_V3
    protected System.Threading.CancellationToken CancellationToken => TestContext.Current.CancellationToken;
    protected ITestOutputHelper? Output => TestContext.Current.TestOutputHelper;
#endif
}