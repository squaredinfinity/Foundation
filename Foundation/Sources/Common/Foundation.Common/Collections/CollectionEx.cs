﻿using SquaredInfinity.Foundation.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Collections
{
    public class CollectionEx<TItem> : 
        Collection<TItem>,
        ICollectionEx<TItem>,
        IBulkUpdatesCollection<TItem>, 
        INotifyCollectionContentChanged
    {
        readonly protected ILock CollectionLock = new ReaderWriterLockSlimEx(LockRecursionPolicy.SupportsRecursion);

        public CollectionEx()
        { }

        public CollectionEx(IList<TItem> items)
            : base(items)
        { }

        protected override void ClearItems()
        {
            using (CollectionLock.AcquireWriteLock())
            {
                base.ClearItems();

                OnVersionChangedInternal();
            }
        }

        protected override void InsertItem(int index, TItem item)
        {
            using (CollectionLock.AcquireWriteLock())
            {
                base.InsertItem(index, item);

                OnVersionChangedInternal();
            }
        }

        protected override void RemoveItem(int index)
        {
            using (CollectionLock.AcquireWriteLock())
            {
                base.RemoveItem(index);

                OnVersionChangedInternal();
            }
        }

        protected override void SetItem(int index, TItem item)
        {
            using (CollectionLock.AcquireWriteLock())
            {
                base.SetItem(index, item);

                OnVersionChangedInternal();
            }
        }

        public virtual void AddRange(IEnumerable<TItem> items)
        {
            using(CollectionLock.AcquireWriteLock())
            {
                foreach (var item in items)
                    Items.Add(item);

                OnVersionChangedInternal();
            }
        }

        public virtual void RemoveRange(int index, int count)
        {
            using(CollectionLock.AcquireWriteLock())
            {
                for (int i = index; i < index + count; i++)
                    Items.RemoveAt(index);

                OnVersionChangedInternal();
            }
        }

        public virtual void Replace(TItem oldItem, TItem newItem)
        {
            using(CollectionLock.AcquireUpgradeableReadLock())
            {
                var oldIndex = IndexOf(oldItem);

                if (oldIndex < 0)
                    return;

                SetItem(oldIndex, newItem);
            }
        }

        public virtual void Reset(IEnumerable<TItem> newItems)
        {
            using(CollectionLock.AcquireWriteLock())
            {
                Items.Clear();

                OnVersionChangedInternal();
            }
        }

        void OnVersionChangedInternal()
        {
            OnVersionChanged();
        }

        protected virtual void OnVersionChanged() 
        {
        }

        public event EventHandler<CollectionContentChangedEventArgs> VersionChanged;

        int _version;
        public int Version
        {
            get { return _version; }
        }
    }
}
