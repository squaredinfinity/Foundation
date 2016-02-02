using System;
using System.Collections.Generic;
using System.Text;

namespace Experimental.Collections
{
    public class ObjectPool<TItem>
    {
        readonly IObjectPoolStore<TItem> Store;

        public ObjectPool(IObjectPoolStore<TItem> store)
        {
            this.Store = store;
        }

        public IObjectPoolItemAcquisition<TItem> Acquire()
        {
            return Store.Acquire();
        }
    }
}
