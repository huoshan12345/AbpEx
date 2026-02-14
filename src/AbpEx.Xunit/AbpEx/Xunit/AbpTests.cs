using System.Threading;

namespace AbpEx.Xunit;

public abstract class AbpTests<TModule> where TModule : AbpModule
{
    private readonly Lazy<IServiceProvider> _lazy;
    protected readonly ITestOutputHelper _output;

#if ABPEX_XUNIT_V3
    protected CancellationToken CancellationToken => TestContext.Current.CancellationToken;
#endif

    protected AbpTests(ITestOutputHelper output)
    {
        _output = output;
        _lazy = new(Initialize);
    }

    public IServiceProvider ServiceProvider => _lazy.Value;

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
                builder.AddXunitTest(_output, true);
            });

        ValidateServices(services);

        var provider = UseAbpAsync
            ? SynchronizationContextScope.Run(services.UseAbpAsync)
            : services.UseAbp();

        var logger = provider.CreateLogger("AbpEx.Xunit");
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