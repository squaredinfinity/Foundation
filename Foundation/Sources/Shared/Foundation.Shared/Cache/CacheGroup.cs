using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Cache
{
    public class CacheGroup : ICacheService
    {
        readonly ICacheService Owner;

        public bool IsTransient { get; private set; }

        readonly string GroupName;

        public CacheGroup(ICacheService owner)
            : this(owner, Guid.NewGuid().ToString())
        {
            IsTransient = true;
        }

        public CacheGroup(ICacheService owner, string groupName)
        {
            this.Owner = owner;
            this.GroupName = groupName + ".";

            _isCacheEnabled = true;
        }

        bool _isCacheEnabled;
        public bool IsCacheEnabled
        {
            get
            {
                if (!Owner.IsCacheEnabled)
                    return false;

                return _isCacheEnabled;
            }
            set
            {
                _isCacheEnabled = value;
            }
        }

        public void Remove(string key)
        {
            Owner.Remove(GroupName + key);
        }

        public T GetOrAdd<T>(string key, Func<T> valueFactory)
        {
            return Owner.GetOrAdd(GroupName + key, valueFactory);
        }

        public T GetOrAdd<T>(string key, Func<T> valueFactory, DateTimeOffset absoluteExpiration)
        {
            return Owner.GetOrAdd<T>(GroupName + key, valueFactory, absoluteExpiration);
        }

        public T GetOrAdd<T>(string key, Func<T> valueFactory, TimeSpan slidingExpiration)
        {
            return Owner.GetOrAdd<T>(GroupName + key, valueFactory, slidingExpiration);
        }

        public T GetOrAdd<T>(string key, Func<T> valueFactory, Predicate<ICacheItemDetails<T>> shouldForceCacheExpiration)
        {
            return Owner.GetOrAdd<T>(GroupName + key, valueFactory, shouldForceCacheExpiration);
        }

        public T GetOrAdd<T>(string key, Func<T> valueFactory, TimeSpan slidingExpiration, Predicate<ICacheItemDetails<T>> shouldForceCacheExpiration)
        {
            return Owner.GetOrAdd<T>(GroupName + key, valueFactory, slidingExpiration, shouldForceCacheExpiration);
        }

        public T GetOrAdd<T>(string key, Func<T> valueFactory, DateTimeOffset absoluteExpiration, Predicate<ICacheItemDetails<T>> shouldForceCacheExpiration)
        {
            return Owner.GetOrAdd<T>(GroupName + key, valueFactory, absoluteExpiration, shouldForceCacheExpiration);
        }

        public void ClearAll()
        {
            Owner.ClearAll();
        }

        public ICacheService NewTransientCacheGroup()
        {
            return new CacheGroup(this);
        }

        public ICacheService NewCacheGroup(string groupName)
        {
            return new CacheGroup(this, groupName);
        }
    }
}