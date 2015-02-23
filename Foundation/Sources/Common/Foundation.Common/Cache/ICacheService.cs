using System;
using System.Runtime.Caching;
namespace SquaredInfinity.Foundation.Cache
{
    public interface ICacheService
    {
        bool IsCacheEnabled { get; set; }

        void Remove(string key);

        T GetOrAdd<T>(string key, Func<T> valueFactory);
        T GetOrAdd<T>(string key, Func<T> valueFactory, Predicate<ICacheItemDetails<T>> shouldForceCacheExpiration);
        T GetOrAdd<T>(string key, Func<T> valueFactory, TimeSpan slidingExpiration, Predicate<ICacheItemDetails<T>> shouldForceCacheExpiration);
        T GetOrAdd<T>(string key, Func<T> valueFactory, DateTimeOffset absoluteExpiration, Predicate<ICacheItemDetails<T>> shouldForceCacheExpiration);
        T GetOrAdd<T>(string key, Func<T> valueFactory, DateTimeOffset absoluteExpiration);
        T GetOrAdd<T>(string key, Func<T> valueFactory, TimeSpan slidingExpiration);
        T GetOrAdd<T>(string key, Func<T> valueFactory, CacheItemPolicy cacheItemPolicy);

    }
}