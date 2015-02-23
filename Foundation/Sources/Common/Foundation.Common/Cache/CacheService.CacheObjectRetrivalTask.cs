using System;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Cache
{
    public partial class CacheService
    {
        class CacheObjectRetrivalTask : Task<object>
        {
            public CacheObjectRetrivalTask(Func<object> getCacheItem)
                : base(getCacheItem)
            { }
        }
    }
}