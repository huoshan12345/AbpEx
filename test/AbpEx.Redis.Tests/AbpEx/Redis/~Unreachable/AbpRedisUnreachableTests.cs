namespace AbpEx.Redis;

public class AbpRedisUnreachableTests : AbpTests<AbpRedisTestModule>
{
    protected override IConfigurationRoot BuildConfig()
    {
        return new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.Unreachable.json", false, false)
            .Build();
    }
}