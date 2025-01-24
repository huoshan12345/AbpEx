using System;
using System.Collections.Generic;
using EasyCaching.Core;
using AbpEx.Caching.Configuration;

namespace AbpEx.Caching;

public interface ICacheManager : IDisposable
{
    IReadOnlyCacheOptions CacheOptions { get; }
    IReadOnlyList<ICache> GetAllCaches();
    ICache<T> GetCache<T>(string name);
    ICache GetCache(string name);
    ProviderInfo ProviderInfo { get; }
}