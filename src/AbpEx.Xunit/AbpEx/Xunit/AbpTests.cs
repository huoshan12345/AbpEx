namespace AbpEx.Xunit;

public abstract class AbpTests<TModule> where TModule : AbpModule
{
    private readonly Lazy<IServiceProvider> _lazy;

#if ABPEX_XUNIT_V3
    protected System.Threading.CancellationToken CancellationToken => TestContext.Current.CancellationToken;
    protected ITestOutputHelper? Output => TestContext.Current.TestOutputHelper;

    protected AbpTests()
    {
        _lazy = new(Initialize);
    }
#else
    protected readonly ITestOutputHelper _output;

    protected AbpTests(ITestOutputHelper output)
    {
        _output = output;
        _lazy = new(Initialize);
    }
#endif

    public IServiceProvider ServiceProvider => _lazy.Value;
    public ILogger Logger => ServiceProvider.CreateLogger(GetType());

    protected IServiceProvider Initialize()
    {
        var watch = ValueStopwatch.StartNew();

        var config = BuildConfig();
        var services = new ServiceCollection()
            .AddSingleton<IConfigurationRoot>(config)
            .AddSingleton<IConfiguration>(config)
            .AddAbp<TModule>(m => Configure(m, config))
            .AddLogging(builder =>
            {
                builder.SetMinimumLevel(LogLevel);
#if ABPEX_XUNIT_V3
                builder.AddXunit();
#else
                builder.AddXunit(_output, true);
#endif
            });

        ValidateServices(services);

        var provider = UseAbpAsync
            ? SynchronizationContextScope.Run(services.UseAbpAsync)
            : services.UseAbp();

        var logger = provider.CreateLogger(GetType());
        logger.LogDebug("It takes {ElapsedSeconds:f3} seconds to initialize abp framework", watch.GetElapsedTime().TotalSeconds);
        return provider;
    }

    protected virtual void ValidateServices(IServiceCollection services)
    {
        var options = new ServiceProviderOptions
        {
            ValidateScopes = ValidateScopes,
            ValidateOnBuild = ValidateOnBuild
        };

        services.BuildServiceProvider(options);
    }

    protected virtual void Configure(AbpApplicationCreationOptions options, IConfigurationRoot configuration)
    {
    }

    protected virtual bool UseAbpAsync { get; } = true;
    protected virtual LogLevel LogLevel => LogLevel.Trace;
    protected virtual bool ValidateOnBuild { get; } = true;
    protected virtual bool ValidateScopes { get; } = true;

    protected virtual IConfigurationRoot BuildConfig()
    {
        return new ConfigurationBuilder().Build();
    }
}