using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.Foundation.Collections
{
    public class DefaultObjectPoolStore<TItem> : IObjectPoolStore<TItem>
    {
        readonly IProducerConsumerCollection<TItem> InternalCollection;
        readonly IReadOnlyList<TItem> InitialItems;

        Func<TItem> ConstructTransientItem { get; set; }

        public DefaultObjectPoolStore(DefaultObjectPoolStoreMode mode, IReadOnlyList<TItem> items, Func<TItem> constructTransientItem)
        {
            switch (mode)
            {
                case DefaultObjectPoolStoreMode.Stack:
                    InternalCollection = new ConcurrentStack<TItem>(items);
                    break;
                case DefaultObjectPoolStoreMode.Queue:
                    InternalCollection = new ConcurrentQueue<TItem>(items);
                    break;
                default:
                    throw new NotSupportedException();
            }

            this.InitialItems = items;
        }

        public DefaultObjectPoolStore(IProducerConsumerCollection<TItem> internalCollection, Func<TItem> constructTransientItem)
        {
            this.InternalCollection = internalCollection;
            this.ConstructTransientItem = constructTransientItem;

            this.InitialItems = InternalCollection.ToArray();
        }

        public IObjectPoolItemAcquisition<TItem> Acquire()
        {
            TItem item = default(TItem);

            if (InternalCollection.TryTake(out item))
            {
                return new ObjectPoolItemAcquisition<TItem>(this, item, isItemTransient: false);
            }
            else
            {
                return new ObjectPoolItemAcquisition<TItem>(this, ConstructTransientItem(), isItemTransient: true);
            }
        }

        public void Release(IObjectPoolItemAcquisition<TItem> item)
        {
            if (item.IsItemTransient)
                return;

            InternalCollection.TryAdd(item.Item);
        }
    }
}
