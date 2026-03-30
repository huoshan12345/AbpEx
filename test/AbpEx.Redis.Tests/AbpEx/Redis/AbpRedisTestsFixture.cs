namespace AbpEx.Redis;

public class AbpRedisTestsFixture : AbpTestsFixture<AbpRedisTestsModule>
{
    protected override bool UseAop => true;

    protected override IConfiguration BuildConfig()
    {
        return GlobalConstants.Config;
    }
}