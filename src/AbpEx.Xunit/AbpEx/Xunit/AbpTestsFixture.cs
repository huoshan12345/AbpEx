namespace AbpEx.Xunit;

public class AbpTestsFixture<TModule> : IAsyncLifetime where TModule : AbpModule
{
    private readonly Lazy<IServiceProvider> _services;
    public IServiceProvider Services => _services.Value;

#if ABPEX_XUNIT_V3

    public AbpTestsFixture()
    {
        _services = new(Build);
    }
#else
    protected readonly ITestOutputHelper _output;

    protected AbpTestsFixture(ITestOutputHelper output)
    {
        _output = output;
        _services = new(Build);
    }
#endif

    protected virtual IServiceCollection CreateServices()
    {
        var config = BuildConfig();
        var services = new ServiceCollection()
            .AddSingleton(config)
            .AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.SetMinimumLevel(LogLevel);
#if ABPEX_XUNIT_V3
                builder.AddXunit();
#else
                builder.AddXunit(_output, true);
#endif
            })
            .AddAbp<TModule>();

        if (UseAop)
        {
            services.AddAop();
        }

        return services;
    }

    protected virtual IServiceProvider Build()
    {
        var services = CreateServices();
        return services.BuildServiceProviderFromFactory();
    }

    protected virtual bool UseAop => false;
    protected virtual LogLevel LogLevel => LogLevel.Trace;

    public
#if ABPEX_XUNIT_V3
        ValueTask 
#else
        Task
#endif
        DisposeAsync()
    {
        GC.SuppressFinalize(this);

        if (_services.IsValueCreated)
            Disposable.FromValue(Services).Dispose();
#if ABPEX_XUNIT_V3
        return new ValueTask(Task.CompletedTask);
#else
        return Task.CompletedTask;
#endif
    }

    public async
#if ABPEX_XUNIT_V3
        ValueTask 
#else
        Task
#endif
        InitializeAsync()
    {
        await Services.UseAbpAsync();
    }

    protected virtual IConfiguration BuildConfig()
    {
        return new ConfigurationBuilder().Build();
    }
}