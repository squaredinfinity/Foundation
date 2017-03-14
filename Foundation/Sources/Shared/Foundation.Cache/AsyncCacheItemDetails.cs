using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Cache
{
    public class AsyncCacheItemDetails<T> : ICacheItemDetails<T>
    {
        readonly Task<T> ItemFactoryTask;
        public T Value
        {
            get { return ItemFactoryTask.Result; }
        }
        public DateTime TimeAddedToCacheUtc { get; set; }

        public AsyncCacheItemDetails(Func<T> itemFactory)
        {
            ItemFactoryTask = new Task<T>(itemFactory);
            ItemFactoryTask.Start();
        }
    }

}
