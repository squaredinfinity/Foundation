using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.Foundation.Collections
{
    class Stack<TItem> : IProducerConsumerCollection<TItem>, IObjectPoolStoreCollection<TItem>
    {
        public readonly List<TItem> InternalStorage = new List<TItem>();

        public int Count
        {
            get { return InternalStorage.Count; }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        readonly object _syncRoot = new object();
        public object SyncRoot
        {
            get { return _syncRoot; }
        }

        public Stack()
        { }

        public Stack(IReadOnlyList<TItem> items)
        {
            InternalStorage.AddRange(items);
        }

        public void CopyTo(Array array, int index)
        {
            var array_t = (TItem[])array;
            InternalStorage.CopyTo(array_t, index);
        }

        public void CopyTo(TItem[] array, int index)
        {
            InternalStorage.CopyTo(array, index);
        }

        public IEnumerator<TItem> GetEnumerator()
        {
            return InternalStorage.GetEnumerator();
        }

        public TItem[] ToArray()
        {
            return InternalStorage.ToArray();
        }

        public bool TryAdd(TItem item)
        {
            return TryPush(item);
        }

        public bool TryTake(out TItem item)
        {
            return TryPop(out item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return InternalStorage.GetEnumerator();
        }

        public void Push(TItem item)
        {
            lock(SyncRoot)
            {
                InternalStorage.Add(item);
            }
        }

        public TItem Pop()
        {
            lock(SyncRoot)
            {
                var ix = InternalStorage.Count - 1;
                var item = InternalStorage[ix];
                InternalStorage.RemoveAt(ix);
                return item;
            }
        }

        public bool TryPush(TItem item)
        {
            lock(SyncRoot)
            {
                InternalStorage.Add(item);
                return true;
            }
        }

        public bool TryPop(out TItem item)
        {
            lock(SyncRoot)
            {
                if (Count == 0)
                {
                    item = default(TItem);
                    return false;
                }

                var ix = InternalStorage.Count - 1;
                item = InternalStorage[ix];
                InternalStorage.RemoveAt(ix);
                return true;
            }
        }

        public bool TryRemove(TItem item)
        {
            lock(SyncRoot)
            {
                for(int i = 0; i < InternalStorage.Count; i++)
                {
                    if(object.Equals(item, InternalStorage[i]))
                    {
                        InternalStorage.RemoveAt(i);
                        return true;
                    }
                }
            }

            return false;
        }
    }

    public interface IObjectPoolStoreCollection<TItem> : IProducerConsumerCollection<TItem>
    {
        bool TryRemove(TItem item);
    }

    public class DefaultObjectPoolStore<TItem> : IObjectPoolStore<TItem>
    {
        readonly IObjectPoolStoreCollection<TItem> InternalCollection;

        Func<TItem> ConstructTransientItem { get; set; }

        public DefaultObjectPoolStore(DefaultObjectPoolStoreMode mode, IReadOnlyList<TItem> items, Func<TItem> constructTransientItem)
        {
            switch (mode)
            {
                case DefaultObjectPoolStoreMode.Stack:
                    InternalCollection = new Stack<TItem>(items);
                    break;
                case DefaultObjectPoolStoreMode.Queue:
                    //InternalCollection = new ConcurrentQueue<TItem>(items);
                    throw new NotImplementedException();
                    break;
                default:
                    throw new NotSupportedException();
            }

            ConstructTransientItem = constructTransientItem;
        }

        public DefaultObjectPoolStore(IObjectPoolStoreCollection<TItem> internalCollection, Func<TItem> constructTransientItem)
        {
            this.InternalCollection = internalCollection;
            this.ConstructTransientItem = constructTransientItem;
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

        public bool TryAcquireItem(TItem item, out IObjectPoolItemAcquisition<TItem> acquisition)
        {
            acquisition = null;

            if(InternalCollection.TryRemove(item))
            {
                acquisition = new ObjectPoolItemAcquisition<TItem>(this, item, isItemTransient: false);
                return true;
            }

            return false;
        }
    }
}
