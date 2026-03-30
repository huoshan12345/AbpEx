namespace AbpEx.Redis;

public class AbpRedisUnreachableTestsFixture : AbpTestsFixture<AbpRedisTestsModule>
{
    protected override bool UseAop => true;

    protected override IConfiguration BuildConfig()
    {
        return new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.Unreachable.json", false, false)
            .Build();
    }
}