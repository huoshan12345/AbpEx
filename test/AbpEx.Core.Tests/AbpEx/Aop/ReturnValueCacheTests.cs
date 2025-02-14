using Meziantou.Xunit;
using xRetry;

namespace AbpEx.Aop;

public class ReturnValueCacheTests(ITestOutputHelper output) : AbpAopTests<AbpTestModule>(output)
{
    public static readonly TimeSpan CacheMaxTime = TimeSpan.FromMilliseconds(50);
    public static readonly TimeSpan SleepTime = TimeSpan.FromMilliseconds(100);

    public static IEnumerable<object[]> Numbers { get; } = new[] { -1, 0, 1, 10 }
        .Select(m => new object[] { m }).ToArray();

    protected override void Configure(AbpApplicationCreationOptions options, IConfigurationRoot configuration)
    {
        base.Configure(options, configuration);
        options.Services.AddTransient<IService, Service>();
    }

    public class Model
    {
        public string Id { get; }
        public Model(string id) { Id = id; }
    }

    public interface IService
    {
        int Id { get; }

        [ReturnValueCache(IsStatic = true)]
        Model GetStatic(int id);

        [ReturnValueCache]
        Model Get(int id);
    }

    public class Service : IService
    {
        private static int _id = short.MinValue;
        public int Id { get; }

        public Service()
        {
            Id = Interlocked.Increment(ref _id);
        }

        public Model GetStatic(int id)
        {
            Thread.Sleep(SleepTime);
            return new Model(id.ToString());
        }

        public Model Get(int id)
        {
            Thread.Sleep(SleepTime);
            return new Model($"{_id}_{id}");
        }

        public override int GetHashCode()
        {
            return Id;
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
    public void SameInstance_Test(int no)
    {
        var service = ServiceProvider.GetRequiredService<IService>();
        var itemFromStatic = service.GetStatic(no);
        var itemFromInstance = service.Get(no);

        for (var i = 0; i < 2; i++)
        {
            var (_, tempItem, ex, t) = Operation.Execute(() => service.Get(no));
            Assert.Null(ex);
            Assert.NotNull(tempItem);
            Assert.Equal(itemFromInstance.Id, tempItem.Id);
            Assert.True(t < CacheMaxTime, t.ToString());
        }
        for (var i = 0; i < 2; i++)
        {
            var (_, tempItem, ex, t) = Operation.Execute(() => service.GetStatic(no));
            Assert.Null(ex);
            Assert.NotNull(tempItem);
            Assert.Equal(itemFromStatic.Id, tempItem.Id);
            Assert.True(t < CacheMaxTime, t.ToString());
        }
    }


    private static int _errorCount;

    [DisableParallelization]
    [RetryFact]
    public void Error_Test()
    {
        Interlocked.Increment(ref _errorCount);
        _output.WriteLine("Current error count: " + _errorCount);

        Assert.True(_errorCount >= 3); // Retry 3 times
    }

    [DisableParallelization] // this test rely on the instance id so it should be run in sequence
    [RetryTheory]
    [MemberData(nameof(Numbers))]
    public void DifferentInstance_Test(int no)
    {
        var service = ServiceProvider.GetRequiredService<IService>();
        var itemFromStatic = service.GetStatic(no);

        for (var i = 0; i < 2; i++)
        {
            var tempService = ServiceProvider.GetRequiredService<IService>();

            var (_, fromStatic, _, timeFromStatic) = Operation.Execute(() => tempService.GetStatic(no));
            var (_, fromInstance, _, timeFromInstance) = Operation.Execute(() => tempService.Get(no));

            Assert.NotNull(fromStatic);
            Assert.Equal(itemFromStatic.Id, fromStatic.Id);

            Assert.NotNull(fromInstance);
            Assert.Equal($"{tempService.Id}_{no}", fromInstance.Id);

            Assert.True(timeFromStatic < CacheMaxTime, timeFromStatic.ToString());
            Assert.True(timeFromInstance > SleepTime, timeFromInstance.ToString());
        }
    }
}