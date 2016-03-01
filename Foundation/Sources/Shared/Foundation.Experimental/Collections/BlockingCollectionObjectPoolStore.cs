using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Concurrent;

namespace SquaredInfinity.Foundation.Collections
{
    public class BlockingCollectionObjectPoolStore<TItem> : IObjectPoolStore<TItem>
    {
        BlockingCollection<TItem> InternalCollection;

        Func<TItem> ConstructTransientItem { get; set; }

        public BlockingCollectionObjectPoolStore(IProducerConsumerCollection<TItem> internalCollection, Func<TItem> constructTransientItem)
        {
            this.InternalCollection = new BlockingCollection<TItem>(internalCollection);
            this.ConstructTransientItem = constructTransientItem;
        }

        public IObjectPoolItemAcquisition<TItem> Acquire()
        {
            var raw_item = default(TItem);
            var acquisition = (IObjectPoolItemAcquisition<TItem>)null;

            // no items and can construct transient item
            if (InternalCollection.Count == 0 && ConstructTransientItem != null)
            {
                raw_item = ConstructTransientItem();
                acquisition = new ObjectPoolItemAcquisition<TItem>(this, raw_item, isItemTransient: true);
            }
            else // there are items
            {
                raw_item = InternalCollection.Take();
                acquisition = new ObjectPoolItemAcquisition<TItem>(this, raw_item, isItemTransient: false);
            }

            return acquisition;
        }

        public void Release(IObjectPoolItemAcquisition<TItem> item)
        {
            InternalCollection.Add(item.Item);
        }
    }
}
