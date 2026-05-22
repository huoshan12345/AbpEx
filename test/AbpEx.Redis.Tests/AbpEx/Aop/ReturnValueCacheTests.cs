using AbpEx.Redis;
using AspectCore.DynamicProxy;

namespace AbpEx.Aop;

public class ReturnValueCacheTests(RedisTestsFixture fixture) : RedisTests(fixture)
{
    public static IEnumerable<object[]> Numbers { get; } = new[] { -1, 0, 1, 10 }
        .Select(m => new object[] { m }).ToArray();

    public const int CacheMaxMilliseconds = 100;
    public const int SleepMilliseconds = RedisService.SleepMilliseconds;

    [Fact]
    public void Aop_Test()
    {
        var service = Services.GetRequiredService<IRedisService>();
        Assert.IsNotType<RedisService>(service);
        Assert.True(service.IsProxy());
    }

    [RetryTheory]
    [MemberData(nameof(Numbers))]
    public void IsStatic_SameObject_Test(int no)
    {
        var name = nameof(IsStatic_SameObject_Test) + no;
        var service = Services.GetRequiredService<IRedisService>();
        var itemFromStatic = service.GetStatic(name);

        var (_, tempItem, _, t) = Operation.Execute(() => service.GetStatic(name));
        Assert.NotNull(tempItem);
        Assert.Equal(itemFromStatic.Id, tempItem.Id);
        Assert.True(t.TotalMilliseconds < CacheMaxMilliseconds, t.TotalSeconds.ToString(CultureInfo.InvariantCulture));
    }

    [RetryTheory]
    [MemberData(nameof(Numbers))]
    public void IsStatic_DiffObject_Test(int no)
    {
        var name = nameof(IsStatic_DiffObject_Test) + no;
        var service = Services.GetRequiredService<IRedisService>();
        var itemFromStatic = service.GetStatic(name);

        var tempService = Services.GetRequiredService<IRedisService>();
        var (_, fromStatic, _, t) = Operation.Execute(() => tempService.GetStatic(name));
        Assert.NotNull(fromStatic);
        Assert.Equal(itemFromStatic.Id, fromStatic.Id);
        Assert.True(t.TotalMilliseconds < CacheMaxMilliseconds, t.TotalSeconds.ToString(CultureInfo.InvariantCulture));
    }


    [RetryTheory]
    [MemberData(nameof(Numbers))]
    public void NotStatic_SameObject_Test(int no)
    {
        var name = nameof(NotStatic_SameObject_Test) + no;
        var service = Services.GetRequiredService<IRedisService>();
        var fromInstance = service.Get(name);

        var (_, tempItem, _, t) = Operation.Execute(() => service.Get(name));
        Assert.NotNull(tempItem);
        Assert.Equal(fromInstance.Id, tempItem.Id);
        Assert.True(t.TotalMilliseconds < CacheMaxMilliseconds, t.TotalSeconds.ToString(CultureInfo.InvariantCulture));
    }

    [RetryTheory]
    [MemberData(nameof(Numbers))]
    public void NotStatic_DiffObject_Test(int no)
    {
        var name = nameof(NotStatic_DiffObject_Test) + no;
        var service = Services.GetRequiredService<IRedisService>();
        var fromInstance = service.Get(name);

        var tempService = Services.GetRequiredService<IRedisService>();
        var (_, temp, _, t) = Operation.Execute(() => tempService.Get(name));
        Assert.NotNull(temp);
        Assert.NotEqual(fromInstance.Id, temp.Id);
        Assert.Equal($"{tempService.Id}_{name}", temp.Id);
        Assert.True(t.TotalMilliseconds > SleepMilliseconds, t.TotalSeconds.ToString(CultureInfo.InvariantCulture));
    }
}