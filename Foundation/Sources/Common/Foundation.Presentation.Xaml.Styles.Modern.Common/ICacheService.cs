using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

{
    public interface ICacheService
    {
        bool IsCacheEnabled { get; set; }

        T GetOrAdd<T>(string key, Func<T> valueFactory);

        T GetOrAdd<T>(string key, Func<T> valueFactory, CacheItemPolicy cacheItemPolicy);

        void Remove(string key);

        void AddOrUpdate<T>(string key, T value);

        void AddOrUpdate<T>(string key, T value, Func<string, T, T> updateValueFactory = null);

        void ClearAll();

        /// <summary>
        /// Cache group that is not persisted between application runs
        /// </summary>
        /// <returns></returns>
        ICacheService NewTransientCacheGroup();

        /// <summary>
        /// Cache group that can be persisted between application runs
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        ICacheService NewCacheGroup(string groupName);
    }
}
