using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;

namespace SquaredInfinity.Foundation.Collections
{
    public partial class ObservableCollectionEx<TItem> : IBulkUpdatesCollection<TItem>
    {
        readonly ReaderWriterLockSlim UpdateLock = new ReaderWriterLockSlim();

        public void AddRange(params TItem[] items)
        {
            if (items == null)
                throw new ArgumentNullException("items");
            else
                this.AddRange((IEnumerable<TItem>)items);
        }

        public void AddRange(IEnumerable<TItem> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }
            else
            {
                CollectionLock.EnterWriteLock();

                try
                {
                    List<TItem> list = new List<TItem>(items);

                    for (int i = 0; i < list.Count; i++)
                    {
                        var item = list[i];

                        Items.Add(item);

                        RaiseCollectionChanged(NotifyCollectionChangedAction.Add, (object)item, Items.Count - 1);
                    }
                }
                finally
                {
                    CollectionLock.ExitWriteLock();
                }
            }
        }

        public void RemoveRange(int index, int count)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException("index");
            else if (count < 0)
                throw new ArgumentOutOfRangeException("count");
            else if (index + count > this.Count)
            {
                throw new ArgumentException("number of items to remove is greater than total number of items in the collection");
            }
            else
            {
                CollectionLock.EnterWriteLock();

                try
                {
                    TItem[] objArray = new TItem[count];

                    for (int index1 = 0; index1 < count; index1++)
                    {
                        TItem item = Items[index];
                        objArray[index1] = item;
                        Items.RemoveAt(index);

                        RaiseCollectionChanged(NotifyCollectionChangedAction.Remove, (object)item, index);
                    }
                }
                finally
                {
                    CollectionLock.ExitWriteLock();
                }
            }
        }

        public void Reset(IEnumerable<TItem> newItems)
        {
            CollectionLock.EnterWriteLock();
            try
            {
                bool hadAnyItemsBefore = Items.Count > 0;
                bool hasAnyNewItems = newItems != null && newItems.Any();

                TItem[] oldItems = new TItem[Items.Count];
                Items.CopyTo(oldItems, 0);

                if (hadAnyItemsBefore)
                    Items.Clear();

                if (hasAnyNewItems)
                {
                    foreach (TItem obj in newItems)
                    {
                        this.Items.Add(obj);
                    }
                }

                if (hadAnyItemsBefore || hasAnyNewItems)
                {
                    RaiseCollectionReset();
                }
            }
            finally
            {
                CollectionLock.ExitWriteLock();
            }
        }
    }
}
