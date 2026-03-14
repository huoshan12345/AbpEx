namespace AbpEx.Aop;

public class LoginAndRetryAttributeTests : AbpAopTests<AbpTestModule>
{
    protected override void Configure(AbpApplicationCreationOptions options, IConfigurationRoot configuration)
    {
        base.Configure(options, configuration);
        options.Services.AddUserClient<LoginAndRetryClient>();
    }

    private LoginAndRetryClient CreateClient()
    {
        var account = new UserAccount("test", "test");
        var factory = ServiceProvider.GetRequiredService<IUserClientFactory<LoginAndRetryClient>>();
        var client = factory.Create(account);
        Assert.IsType<LoginAndRetryClient>(client, false);
        return client;
    }

    [Fact]
    public void Aop_Test()
    {
        var factory = ServiceProvider.GetRequiredService<IUserClientFactory<LoginAndRetryClient>>();
        var client = factory.Create(new UserAccount("user", "password"));
        Assert.IsNotType<LoginAndRetryClient>(client);
        Assert.True(client.IsProxy());
    }

    [Fact]
    public async Task Login_Test()
    {
        var account = new UserAccount("user", "password");
        var factory = ServiceProvider.GetRequiredService<IUserClientFactory<LoginAndRetryClient>>();
        var client = factory.Create(account);
        var result = await client.LoginAsync(CancellationToken);
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task ReturnActionEvent_Test()
    {
        var client = CreateClient();
        Assert.False(client.IsOnline);
        var r = await client.DoAsync();
        Assert.True(r.IsSuccess);
        Assert.True(client.IsOnline);
    }

    public class LoginAndRetryClient(ILoggerFactory loggerFactory) : UserClient(loggerFactory: loggerFactory)
    {
        [LoginAndRetry]
        public virtual Task<OperationResult> DoAsync()
        {
            return IsOnline
                ? Operation.Success()
                : Operation.Error("");
        }

        protected override Task<OperationResult> LoginActionAsync(CancellationToken token)
        {
            return Operation.Success();
        }
    }
}