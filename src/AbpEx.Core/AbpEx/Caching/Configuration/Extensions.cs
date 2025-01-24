using System;

namespace AbpEx.Caching.Configuration;

public static class Extensions
{
    public static AbpCacheOptions SetCacheExpireTime(this AbpCacheOptions configuration, string name, TimeSpan timeSpan)
    {
        configuration.Configure(name, o => o.DefaultExpiration = timeSpan);
        return configuration;
    }
}