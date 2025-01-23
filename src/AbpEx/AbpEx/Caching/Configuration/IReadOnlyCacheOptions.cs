using System;

namespace AbpEx.Caching.Configuration;

public interface IReadOnlyCacheOptions
{
    string GlobalPrefix { get; }
    char? Separator { get; }
    TimeSpan DefaultExpiration { get; }
}