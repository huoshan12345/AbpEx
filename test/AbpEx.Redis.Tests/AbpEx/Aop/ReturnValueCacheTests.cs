using System.Xml.Linq;
using AbpEx.Redis;
using AspectCore.DynamicProxy;

// ReSharper disable SpecifyACultureInStringConversionExplicitly

namespace AbpEx.Aop;

public class ReturnValueCacheTests(ITestOutputHelper output)
    : AbpRedisTests(output, o => o.AddTransient<IService, Service>())
{
    public const int CacheMaxMilliseconds = 100;
    public const int SleepMilliseconds = 200;

    public static IEnumerable<object[]> Numbers { get; } = new[] { -1, 0, 1, 10 }
        .Select(m => new object[] { m }).ToArray();

    public class Model(string id)
    {
        public string Id { get; } = id;
    }

    public interface IService
    {
        int Id { get; }

        [ReturnValueCache(IsStatic = true)]
        Model GetStatic(string id);

        [ReturnValueCache]
        Model Get(string id);
    }

    public class Service : IService
    {
        private static int _id = short.MinValue;
        public int Id { get; }

        public Service()
        {
            Id = Interlocked.Increment(ref _id);
        }

        public Model GetStatic(string id)
        {
            Thread.Sleep(SleepMilliseconds);
            return new Model(id);
        }

        public Model Get(string id)
        {
            Thread.Sleep(SleepMilliseconds);
            return new Model($"{Id}_{id}");
        }
    }

    [Fact]
    public void Aop_Test()
    {
        var service = ServiceProvider.GetRequiredService<IService>();
        Assert.IsNotType<Service>(service);
        Assert.True(service.IsProxy());
    }

    [RetryTheory]
    [MemberData(nameof(Numbers))]
    public void IsStatic_SameObject_Test(int no)
    {
        var name = nameof(IsStatic_SameObject_Test) + no;
        var service = ServiceProvider.GetRequiredService<IService>();
        var itemFromStatic = service.GetStatic(name);

        var (_, tempItem, _, t) = Operation.Execute(() => service.GetStatic(name));
        Assert.NotNull(tempItem);
        Assert.Equal(itemFromStatic.Id, tempItem.Id);
        Assert.True(t.TotalMilliseconds < CacheMaxMilliseconds, t.TotalSeconds.ToString());
    }

    [RetryTheory]
    [MemberData(nameof(Numbers))]
    public void IsStatic_DiffObject_Test(int no)
    {
        var name = nameof(IsStatic_DiffObject_Test) + no;
        var service = ServiceProvider.GetRequiredService<IService>();
        var itemFromStatic = service.GetStatic(name);

        var tempService = ServiceProvider.GetRequiredService<IService>();
        var (_, fromStatic, _, t) = Operation.Execute(() => tempService.GetStatic(name));
        Assert.NotNull(fromStatic);
        Assert.Equal(itemFromStatic.Id, fromStatic.Id);
        Assert.True(t.TotalMilliseconds < CacheMaxMilliseconds, t.TotalSeconds.ToString());
    }


    [RetryTheory]
    [MemberData(nameof(Numbers))]
    public void NotStatic_SameObject_Test(int no)
    {
        var name = nameof(NotStatic_SameObject_Test) + no;
        var service = ServiceProvider.GetRequiredService<IService>();
        var fromInstance = service.Get(name);

        var (_, tempItem, _, t) = Operation.Execute(() => service.Get(name));
        Assert.NotNull(tempItem);
        Assert.Equal(fromInstance.Id, tempItem.Id);
        Assert.True(t.TotalMilliseconds < CacheMaxMilliseconds, t.TotalSeconds.ToString());
    }

    [RetryTheory]
    [MemberData(nameof(Numbers))]
    public void NotStatic_DiffObject_Test(int no)
    {
        var name = nameof(NotStatic_DiffObject_Test) + no;
        var service = ServiceProvider.GetRequiredService<IService>();
        var fromInstance = service.Get(name);

        var tempService = ServiceProvider.GetRequiredService<IService>();
        var (_, temp, _, t) = Operation.Execute(() => tempService.Get(name));
        Assert.NotNull(temp);
        Assert.NotEqual(fromInstance.Id, temp.Id);
        Assert.Equal($"{tempService.Id}_{name}", temp.Id);
        Assert.True(t.TotalMilliseconds > SleepMilliseconds, t.TotalSeconds.ToString());
    }
}