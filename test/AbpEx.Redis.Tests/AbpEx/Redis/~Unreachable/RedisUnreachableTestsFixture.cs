namespace AbpEx.Redis;

public class RedisUnreachableTestsFixture : RedisTestsFixture
{
    protected override IConfiguration BuildConfig()
    {
        return new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.unreachable.json", false, false)
            .Build();
    }
}