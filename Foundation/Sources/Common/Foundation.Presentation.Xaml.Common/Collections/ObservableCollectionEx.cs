using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Data;
using System.Windows.Threading;

namespace SquaredInfinity.Foundation.Collections
{
    public partial class ObservableCollectionEx<TItem> : Collection<TItem>
    {
        //readonly object Sync = new object();
        //readonly SynchronizationContext SyncContext;
        readonly ReaderWriterLockSlim CollectionLock = new ReaderWriterLockSlim();

        public ObservableCollectionEx(SynchronizationContext syncContext)
            : this(21, syncContext)
        { }

        public ObservableCollectionEx()
            : this(21, new DispatcherSynchronizationContext())
        { }

        public ObservableCollectionEx(int capacity, SynchronizationContext syncContext)
            : base(new List<TItem>(capacity))
        {
            //SyncContext = syncContext;

            BindingOperations.EnableCollectionSynchronization(this, context: null, synchronizationCallback: BindingSync);
        }

        void BindingSync(IEnumerable collection, object context, Action accessMethod, bool writeAccess)
        {
            if (writeAccess)
            {
                CollectionLock.EnterWriteLock();
            }
            else
            {
                CollectionLock.EnterReadLock();
            }

            try
            {
                accessMethod();
            }
            finally
            {
                if (CollectionLock.IsWriteLockHeld)
                {
                    CollectionLock.ExitWriteLock();
                }
                else if (CollectionLock.IsReadLockHeld)
                {
                    CollectionLock.ExitReadLock();
                }
            }
        }

        public void Move(int oldIndex, int newIndex)
        {
            CollectionLock.EnterWriteLock();
            try
            {
                TItem obj = this[oldIndex];
                base.RemoveItem(oldIndex);
                base.InsertItem(newIndex, obj);

                RaiseCollectionChanged(NotifyCollectionChangedAction.Move, (object)obj, newIndex, oldIndex);
            }
            finally
            {
                CollectionLock.ExitWriteLock();
            }
        }

        protected sealed override void RemoveItem(int index)
        {
            CollectionLock.EnterWriteLock();
            try
            {
                TItem obj = this[index];
                base.RemoveItem(index);
                this.RaiseCollectionChanged(NotifyCollectionChangedAction.Remove, (object)obj, index);
            }
            finally
            {
                CollectionLock.ExitWriteLock();
            }
        }

        protected sealed override void InsertItem(int index, TItem item)
        {
            CollectionLock.EnterWriteLock();
            try
            {
                base.InsertItem(index, item);
                this.RaiseCollectionChanged(NotifyCollectionChangedAction.Add, (object)item, index);
            }
            finally
            {
                CollectionLock.ExitWriteLock();
            }
        }

        protected sealed override void SetItem(int index, TItem item)
        {
            CollectionLock.EnterWriteLock();

            try
            {
                TItem obj = this[index];
                base.SetItem(index, item);
                this.RaiseCollectionChanged(NotifyCollectionChangedAction.Replace, (object)item, (object)obj, index);
            }
            finally
            {
                CollectionLock.ExitWriteLock();
            }
        }

        protected sealed override void ClearItems()
        {
            CollectionLock.EnterWriteLock();

            try
            {
                base.ClearItems();
                RaiseCollectionReset();
            }
            finally
            {
                CollectionLock.ExitWriteLock();
            }
        }
    }
}
