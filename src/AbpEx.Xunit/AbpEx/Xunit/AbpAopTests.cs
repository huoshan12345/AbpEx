namespace AbpEx.Xunit;

public abstract class AbpAopTests<TModule>(ITestOutputHelper output) : AbpTests<TModule>(output)
    where TModule : AbpModule
{
    protected override LogLevel LogLevel => LogLevel.Debug;
    protected override bool ValidateScopes { get; } = false;

    protected override void Configure(AbpApplicationCreationOptions options, IConfigurationRoot configuration)
    {
        options.Services.AddAop();
    }
}