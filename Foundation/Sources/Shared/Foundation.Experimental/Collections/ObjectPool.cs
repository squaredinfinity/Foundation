using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.Collections
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

        public bool TryAcquireItem(TItem item, out IObjectPoolItemAcquisition<TItem> acquisition)
        {
            return Store.TryAcquireItem(item, out acquisition);
        }
    }
}
