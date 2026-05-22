using Volo.Abp.Modularity;

namespace AbpEx;

public class CoreTestsFixture<TModule> : IAsyncLifetime where TModule : AbpModule
{
    private readonly Lazy<IServiceProvider> _services;
    public IServiceProvider Services => _services.Value;
    public ITestOutputHelper? Output => TestContext.Current.TestOutputHelper;
    public CancellationToken CancellationToken => TestContext.Current.CancellationToken;

    protected virtual LogLevel LogLevel => LogLevel.Trace;

    public CoreTestsFixture()
    {
        _services = new(Build);
    }

    protected virtual IServiceCollection CreateServices()
    {
        var config = BuildConfig();
        var services = new ServiceCollection()
            .AddSingleton(config)
            .AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.SetMinimumLevel(LogLevel);
                builder.AddXunit();
            })
            .AddAbp<TModule>()
            .AddAop();

        return services;
    }

    protected virtual IServiceProvider Build()
    {
        var services = CreateServices();
        return services.BuildServiceProviderFromFactory();
    }

    protected virtual IConfiguration BuildConfig()
    {
        return new ConfigurationBuilder().Build();
    }

    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        if (_services.IsValueCreated)
            Disposable.FromValue(Services).Dispose();

        return ValueTask.CompletedTask;
    }

    public async ValueTask InitializeAsync()
    {
        await Services.UseAbpAsync();
    }
}

public class CoreTestsFixture : CoreTestsFixture<CoreTestsTestsModule>;
