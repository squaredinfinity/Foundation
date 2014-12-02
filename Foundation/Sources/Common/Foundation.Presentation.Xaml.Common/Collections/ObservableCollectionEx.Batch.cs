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
                using(var writeLock = CollectionLock.AcquireWriteLock())
                {
                    List<TItem> list = new List<TItem>(items);

                    for (int i = 0; i < list.Count; i++)
                    {
                        var item = list[i];

                        Items.Add(item);

                        OnAfterItemAdded(item);

                        if (MonitorElementsForChanges)
                            BeginItemChangeMonitoring(item);

                        RaiseCollectionChanged(NotifyCollectionChangedAction.Add, (object)item, Items.Count - 1);
                    }
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
                using(var writeLock = CollectionLock.AcquireWriteLock())
                { 
                    TItem[] objArray = new TItem[count];

                    for (int index1 = 0; index1 < count; index1++)
                    {
                        TItem item = Items[index];
                        objArray[index1] = item;
                        Items.RemoveAt(index);

                        OnAfterItemRemoved(item);

                        if (MonitorElementsForChanges)
                            StopItemChangeMonitoring(item);

                        RaiseCollectionChanged(NotifyCollectionChangedAction.Remove, (object)item, index);
                    }
                }
            }
        }

        public void Reset(IEnumerable<TItem> newItems)
        {
            using(var readLock = CollectionLock.AcquireUpgradeableReadLock())
            { 
                bool hadAnyItemsBefore = Items.Count > 0;
                bool hasAnyNewItems = newItems != null && newItems.Any();

                TItem[] oldItems = new TItem[Items.Count];
                Items.CopyTo(oldItems, 0);

                using(readLock.AcquireWriteLock())
                {
                    if (hadAnyItemsBefore)
                    {
                        if(MonitorElementsForChanges)
                        {
                            foreach (var item in Items)
                                StopItemChangeMonitoring(item);
                        }

                        Items.Clear();
                    }

                    if (hasAnyNewItems)
                    {
                        foreach (TItem obj in newItems)
                        {
                            this.Items.Add(obj);

                            if (MonitorElementsForChanges)
                                BeginItemChangeMonitoring(obj);
                        }
                    }
                }

                if (hadAnyItemsBefore || hasAnyNewItems)
                {
                    RaiseCollectionReset();
                }
            }
        }

        const int STATE__NORMAL = 0;
        const int STATE__BULKUPDATE = 1;

        int State = STATE__NORMAL;

        public IBulkUpdate BeginBulkUpdate()
        {
            if (Interlocked.CompareExchange(ref State, STATE__BULKUPDATE, STATE__NORMAL) != STATE__NORMAL)
            {
                throw new Exception("Bulk Update Operation has already started");
            }

            return new BulkUpdate(this);
        }

        public void EndBulkUpdate(IBulkUpdate bulkUpdate)
        {
            bulkUpdate.Dispose();

            if (Interlocked.CompareExchange(ref State, STATE__NORMAL, STATE__BULKUPDATE) != STATE__BULKUPDATE)
            {
                throw new Exception("Bulk Update Operation has already ended");
            }

            RaiseCollectionReset();            
        }

        public void AddRange(IEnumerable items)
        {
            AddRange(items.Cast<TItem>());
        }

        public void Reset(IEnumerable newItems)
        {
            Reset(newItems.Cast<TItem>());
        }

        class BulkUpdate : IBulkUpdate
        {
            readonly IBulkUpdatesCollection Owner;
            bool HasFinished = false;

            public BulkUpdate(IBulkUpdatesCollection owner)
            {
                this.Owner = owner;
            }

            public void Dispose()
            {
                if (HasFinished)
                    return;

                HasFinished = true;
                Owner.EndBulkUpdate(this);
            }
        }
    }
}
