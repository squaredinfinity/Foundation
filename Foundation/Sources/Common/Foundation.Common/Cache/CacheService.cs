    public interface IFeatureMetadata
    {
        // Summary:
        //     Default: int.MaxValue => will be loaded last, after any other resources with
        //     custom Import Order
        [DefaultValue(2147483647)]
        int ImportOrder { get; }

        string BuildQuality { get; }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    [MetadataAttribute]
    public class FeatureExportMetadataAttribute : ExportAttribute, IFeatureMetadata
    {
        public FeatureExportMetadataAttribute() { }

        public int ImportOrder { get; set; }

        public string BuildQuality { get; set; }
    }

    public interface IConvertibleType
    {
        bool CanConvertTo(Type destinationType);
        object ConvertTo(Type destinationType);
    }

    public class ConvertibleTypeConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            var typeConvertible = context.Instance as IConvertibleType;

            return typeConvertible.CanConvertTo(destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            var typeConvertible = value as IConvertibleType;

            return typeConvertible.ConvertTo(destinationType);
        }
    }

       class CacheObjectRetrivalTask : Task<object>
    { 
        public CacheObjectRetrivalTask(Func<object> getCacheItem)
            : base(getCacheItem)
        { }
    }

    public class CacheService : ICacheService
    {
        MemoryCache Cache = new MemoryCache("CacheService");
        
        ConcurrentDictionary<string, ILock> KeyLocks = new ConcurrentDictionary<string, ILock>(concurrencyLevel:64, capacity:2048);

        public bool IsCacheEnabled { get; set; }

        public void Remove(string key)
        {
            Cache.Remove(key);
        }

        public T GetOrAdd<T>(string key, Func<T> valueFactory)
        {
            return GetOrAdd(key, valueFactory, GetDefaultCachePolicy());
        }

        public T GetOrAdd<T>(string key, Func<T> valueFactory, Predicate<ICacheItemDetails<T>> shouldForceCacheExpiration)
        {
            return GetOrAdd<T>(key, shouldForceCacheExpiration, valueFactory, GetDefaultCachePolicy());
        }

        public T GetOrAdd<T>(string key, Func<T> valueFactory, TimeSpan slidingExpiration, Predicate<ICacheItemDetails<T>> shouldForceCacheExpiration)
        {
            var policy = new CacheItemPolicy();
            policy.SlidingExpiration = slidingExpiration;

            return GetOrAdd<T>(key, shouldForceCacheExpiration, valueFactory, policy);
        }

        public T GetOrAdd<T>(string key, Func<T> valueFactory, DateTimeOffset absoluteExpiration, Predicate<ICacheItemDetails<T>> shouldForceCacheExpiration)
        {
            var policy = new CacheItemPolicy();
            policy.AbsoluteExpiration = absoluteExpiration;

            return GetOrAdd<T>(key, shouldForceCacheExpiration, valueFactory, GetDefaultCachePolicy());
        }

        public T GetOrAdd<T>(string key, Func<T> valueFactory, DateTimeOffset absoluteExpiration)
        {
            var policy = new CacheItemPolicy();
            policy.AbsoluteExpiration = absoluteExpiration;

            return GetOrAdd(key, valueFactory, policy);
        }

        public T GetOrAdd<T>(string key, Func<T> valueFactory, TimeSpan slidingExpiration)
        {
            var policy = new CacheItemPolicy();
            policy.SlidingExpiration = slidingExpiration;

            return GetOrAdd(key, valueFactory, policy);
        }

        public T GetOrAdd<T>(string key, Func<T> valueFactory, CacheItemPolicy cacheItemPolicy)
        {
            return GetOrAdd<T>(key, null, valueFactory, cacheItemPolicy);
        }

        public bool TryGet__NOLOCK<T>(string key, out T result)
        {
            // get item from cache
            var cachedObject_candidate = Cache.Get(key);

            var cacheItemDetails = cachedObject_candidate as ICacheItemDetails<T>;

            if(cacheItemDetails == null)
            {
                // item IS NOT in cache
                result = default(T);
                return false;
            }
            else
            {
                // item IS in cache
                result = (T)cacheItemDetails.Value;

                if(result == null)
                    return false;
                else
                    return true;
            }
        }

        public bool TryGetCacheItem__NOLOCK<T>(string key, out ICacheItemDetails<T> result)
        {
            // get item from cache
            var cachedObject_candidate = Cache.Get(key);

            result = cachedObject_candidate as ICacheItemDetails<T>;

            if (result == null)
            {
                // item IS NOT in cache
                return false;
            }
            else
            {
                // item IS in cache
                return true;
            }
        }

        public T GetOrAdd<T>(string key, Predicate<ICacheItemDetails<T>> shouldForceCacheExpiration, Func<T> valueFactory, CacheItemPolicy cacheItemPolicy)
        {
            if (!IsCacheEnabled)
                return valueFactory();

            // we can do without lock if item is in cache and we don't have to check for forced item expiration
            if (shouldForceCacheExpiration == null)
            {
                T result = default(T);

                if (TryGet__NOLOCK<T>(key, out result))
                {
                    return result;
                }

                shouldForceCacheExpiration = (x) => false;
            }
            
            var keyLock = KeyLocks.GetOrAdd(key, (k) => new ReaderWriterLockSlimEx(LockRecursionPolicy.SupportsRecursion));

            // if we must check for forced expiration or item is not in cache we need lock
            using (var readLock = keyLock.AcquireUpgradeableReadLock())
            {
                ICacheItemDetails<T> cacheItem = (ICacheItemDetails<T>)null;
                if (TryGetCacheItem__NOLOCK<T>(key, out cacheItem))
                {
                    // cache item is in cache, check if it should expire
                    var shouldExpire = shouldForceCacheExpiration(cacheItem);

                    if (!shouldExpire)
                    {
                        // exit lock and return item
                        readLock.Dispose();

                        return cacheItem.Value;
                    }
                }

                // get item
                cacheItem = new AsyncCacheItemDetails<T>(valueFactory);
                cacheItem.TimeAddedToCacheUtc = DateTime.UtcNow;

                using (var writeLock = readLock.UpgradeToWriteLock())
                {
                    Cache.Set(key, cacheItem, cacheItemPolicy);
                }

                readLock.Dispose();

                var r = cacheItem.Value;

                return r;
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

            policy.AbsoluteExpiration = DateTimeOffset.Now + TimeSpan.FromMinutes(5);

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
