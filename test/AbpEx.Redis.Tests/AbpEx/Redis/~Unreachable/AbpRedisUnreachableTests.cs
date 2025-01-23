namespace AbpEx.Redis;

public class AbpRedisUnreachableTests(ITestOutputHelper output) : AbpTests<AbpRedisTestModule>(output)
{
    protected override IConfigurationRoot BuildConfig()
    {
        return new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.Unreachable.json", false, false)
            .Build();
    }
}