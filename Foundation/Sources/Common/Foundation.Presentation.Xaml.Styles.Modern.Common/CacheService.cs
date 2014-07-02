using SquaredInfinity.Foundation.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

{
    class CacheObjectRetrivalTask : Task<object>
    { 
        public CacheObjectRetrivalTask(Func<object> getCacheItem)
            : base(getCacheItem)
        { }
    }

    public class CacheService : ICacheService
    {
        MemoryCache Cache = new MemoryCache("CacheService");

        ILock CacheLock = new ReaderWriterLockSlimEx(LockRecursionPolicy.SupportsRecursion);

        public bool IsCacheEnabled { get; set; }

        public void Remove(string key)
        {
            Cache.Remove(key);
        }

        public T GetOrAdd<T>(string key, Func<T> valueFactory)
        {
            return GetOrAdd(key, valueFactory, GetDefaultCachePolicy());
        }

        public T GetOrAdd<T>(string key, Func<T> valueFactory, CacheItemPolicy cacheItemPolicy)
        {
            if (!IsCacheEnabled)
                return valueFactory();

            var cachedObject_candidate = Cache.Get(key);

            var cachedObjectRetrivalTask = cachedObject_candidate as CacheObjectRetrivalTask;

            if (cachedObject_candidate != null && cachedObjectRetrivalTask == null)
                return (T)cachedObject_candidate;

            if(cachedObjectRetrivalTask != null)
            {
                return (T) cachedObjectRetrivalTask.Result;
            }

            if (cachedObject_candidate == null || (cachedObjectRetrivalTask != null && cachedObjectRetrivalTask.IsCompleted))
            {
                var t = (CacheObjectRetrivalTask) null;

                using (CacheLock.AcquireWriteLock())
                {
                    // check if cached object has been added by another thread (before write lock was requested)
                    cachedObject_candidate = Cache.Get(key);

                    if(cachedObject_candidate is CacheObjectRetrivalTask)
                    {
                        t = (cachedObject_candidate as CacheObjectRetrivalTask);
                    }
                    else if (cachedObject_candidate != null)
                    {
                        var cachedObject = (T)cachedObject_candidate;
                        return cachedObject;
                    }

                    if (t == null)
                    {
                        t = new CacheObjectRetrivalTask(() =>
                            {
                                var cachedObject = valueFactory();

                                // if null, just return without adding to cache (null cache values are not supported at the moment)
                                if (cachedObject == null)
                                {
                                    using (CacheLock.AcquireWriteLock())
                                    {
                                        Cache.Remove(key);
                                    }

                                    return cachedObject;
                                }

                                Cache.Set(key, cachedObject, cacheItemPolicy);

                                return cachedObject;
                            });

                        Cache.Set(key, t, new CacheItemPolicy() { AbsoluteExpiration = DateTimeOffset.MaxValue });

                        t.Start();
                    }
                }

                return (T)t.Result;
            }

            // should never get this far
            throw new InvalidOperationException();
        }

        public void AddOrUpdate<T>(string key, T value)
        {
            AddOrUpdate<T>(key, value, (k, oldValue) => value);
        }

        public void AddOrUpdate<T>(string key, T value, Func<string, T, T> updateValueFactory = null)
        {
            if (!IsCacheEnabled)
                return;

            var existingValue = (T)Cache.Get(key);

            if (existingValue == null)
            {
                Cache.Set(key, value, GetDefaultCachePolicy());
            }
            else
            {
                var newValue = updateValueFactory(key, existingValue);
                Cache.Set(key, newValue, GetDefaultCachePolicy());
            }
        }

        public void ClearAll()
        {
            var oldCache = Cache;
            Cache = new MemoryCache("CacheService");

            oldCache.Dispose();
        }

        protected virtual CacheItemPolicy GetDefaultCachePolicy()
        {
            var policy = new CacheItemPolicy();

            policy.AbsoluteExpiration = DateTimeOffset.Now + TimeSpan.FromSeconds(15);

            return policy;
        }

        public ICacheService NewTransientCacheGroup()
        {
            return new CacheGroup(this);
        }

        public ICacheService NewCacheGroup(string groupName)
        {
            return new CacheGroup(this, groupName);
        }

        public CacheService()
            : this(isCacheEnabled : true)
        { }

        public CacheService(bool isCacheEnabled)
        {
            IsCacheEnabled = isCacheEnabled;
        }
    }
}
