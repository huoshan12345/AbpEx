namespace AbpEx.Xunit;

public abstract class AbpAopTests<TModule>
#if ABPEX_XUNIT_V3
    : AbpTests<TModule>
#else
    (ITestOutputHelper output) : AbpTests<TModule>(output)
#endif
    where TModule : AbpModule
{
    protected override LogLevel LogLevel => LogLevel.Debug;
    protected override bool ValidateScopes { get; } = false;

    protected override void Configure(AbpApplicationCreationOptions options, IConfigurationRoot configuration)
    {
        options.Services.AddAop();
    }
}