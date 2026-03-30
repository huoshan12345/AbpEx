namespace AbpEx;

public class AbpExTestsFixture : AbpTestsFixture<AbpExTestsModule>
{
    protected override bool UseAop => true;
}