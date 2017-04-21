using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Cache
{
    public static class CacheItemPolicyExtensions
    {
        public static System.Runtime.Caching.CacheItemPolicy ToSystemRuntimeCachingCacheItemPolicy(this CacheItemPolicy cip)
        {
            return new System.Runtime.Caching.CacheItemPolicy
            {
                AbsoluteExpiration = cip.AbsoluteExpiration,
                SlidingExpiration = cip.SlidingExpiration
            };
        }
    }
}
