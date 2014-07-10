//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.Caching;
//using System.Text;
//using System.Threading.Tasks;


//{
//    // todo: allow to specify default cache item policy for group
//    public class CacheGroup : ICacheService
//    {
//        readonly ICacheService Owner;
        
//        public bool IsTransient { get; private set; }

//        readonly string GroupName;

//        public CacheGroup(ICacheService owner)
//            : this(owner, Guid.NewGuid().ToString())
//        {
//            IsTransient = true;
//        }

//        public CacheGroup(ICacheService owner, string groupName)
//        {
//            this.Owner = owner;
//            this.GroupName = groupName + ".";

//            _isCacheEnabled = true;
//        }

//        bool _isCacheEnabled;
//        public bool IsCacheEnabled
//        {
//            get
//            {
//                if (!Owner.IsCacheEnabled)
//                    return false;

//                return _isCacheEnabled;
//            }
//            set
//            {
//                _isCacheEnabled = value;
//            }
//        }

//        public void Remove(string key)
//        {
//            Owner.Remove(GroupName + key);
//        }

//        public T GetOrAdd<T>(string key, Func<T> valueFactory)
//        {
//            return Owner.GetOrAdd(GroupName + key, valueFactory);
//        }

//        public T GetOrAdd<T>(string key, Func<T> valueFactory, CacheItemPolicy cacheItemPolicy)
//        {
//            return Owner.GetOrAdd(GroupName + key, valueFactory, cacheItemPolicy);
//        }

//        public void AddOrUpdate<T>(string key, T value)
//        {
//            Owner.AddOrUpdate(GroupName + key, value);
//        }

//        public void AddOrUpdate<T>(string key, T value, Func<string, T, T> updateValueFactory = null)
//        {
//            Owner.AddOrUpdate(GroupName + key, value, updateValueFactory);
//        }

//        public void ClearAll()
//        {
//            Owner.ClearAll();
//        }

//        public ICacheService NewTransientCacheGroup()
//        {
//            return new CacheGroup(this);
//        }

//        public ICacheService NewCacheGroup(string groupName)
//        {
//            return new CacheGroup(this, groupName);
//        }
//    }
//}
