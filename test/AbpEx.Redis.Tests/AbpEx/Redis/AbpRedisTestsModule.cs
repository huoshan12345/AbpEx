using AbpEx.Aop;
using Volo.Abp.Modularity;

namespace AbpEx.Redis;

[DependsOn(typeof(AbpExRedisModule), typeof(AbpExTestsModule))]
public class AbpRedisTestsModule : AbpModule
{
    public const string RedisSectionKey = "Redis";

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var config = context.Services.GetConfiguration();
        var options = config.GetRequiredValue<RedisDBOptions>(RedisSectionKey);
        context.Services.Configure<AbpRedisOptions>(m =>
        {
            m.RedisOptions = options;
            m.RedisOptions.Database = Environment.Version.Major;
            m.ConfigureAllCollections(x => x.UseGlobalPrefix = true);
        });

        context.Services.AddTransient<IRedisService, RedisService>();
    }
}

public class RedisModel(string id)
{
    public string Id { get; } = id;
}

public interface IRedisService
{
    int Id { get; }

    [ReturnValueCache(IsStatic = true)]
    RedisModel GetStatic(string id);

    [ReturnValueCache]
    RedisModel Get(string id);
}

public class RedisService : IRedisService
{
    public const int SleepMilliseconds = 200;

    private static int _id = short.MinValue;
    public int Id { get; }

    public RedisService()
    {
        Id = Interlocked.Increment(ref _id);
    }

    public RedisModel GetStatic(string id)
    {
        Thread.Sleep(SleepMilliseconds);
        return new RedisModel(id);
    }

    public RedisModel Get(string id)
    {
        Thread.Sleep(SleepMilliseconds);
        return new RedisModel($"{Id}_{id}");
    }
}