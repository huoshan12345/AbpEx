using EasyCaching.Core.Configurations;

namespace AbpEx.Redis;

public interface IReadOnlyAbpRedisOptions
{
    BaseRedisOptions RedisOptions { get; }
    int Database { get; }
    int Timeout { get; }
}