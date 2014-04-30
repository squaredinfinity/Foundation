using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
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
                if(CollectionLock.IsUpgradeableReadLockHeld)
                {
                    CollectionLock.ExitUpgradeableReadLock();
                }

                if (CollectionLock.IsWriteLockHeld)
                {
                    CollectionLock.ExitWriteLock();
                }
                
                if (CollectionLock.IsReadLockHeld)
                {
                    CollectionLock.ExitReadLock();
                }
            }
        }

        public void Move(int oldIndex, int newIndex)
        {
            CollectionLock.EnterUpgradeableReadLock();

            try
            {
                CollectionLock.EnterWriteLock();

                TItem obj = this[oldIndex];
                base.RemoveItem(oldIndex);
                base.InsertItem(newIndex, obj);

                CollectionLock.ExitWriteLock();

                RaiseCollectionChanged(NotifyCollectionChangedAction.Move, (object)obj, newIndex, oldIndex);
            }
            finally
            {
                if (CollectionLock.IsUpgradeableReadLockHeld)
                {
                    CollectionLock.ExitUpgradeableReadLock();
                }

                if (CollectionLock.IsWriteLockHeld)
                {
                    CollectionLock.ExitWriteLock();
                }

                if (CollectionLock.IsReadLockHeld)
                {
                    CollectionLock.ExitReadLock();
                }
            }
        }

        protected override void RemoveItem(int index)
        {
            CollectionLock.EnterUpgradeableReadLock();

            try
            {
                CollectionLock.EnterWriteLock();

                TItem obj = this[index];
                base.RemoveItem(index);

                CollectionLock.ExitWriteLock();

                this.RaiseCollectionChanged(NotifyCollectionChangedAction.Remove, (object)obj, index);
            }
            finally
            {
                if (CollectionLock.IsUpgradeableReadLockHeld)
                {
                    CollectionLock.ExitUpgradeableReadLock();
                }

                if (CollectionLock.IsWriteLockHeld)
                {
                    CollectionLock.ExitWriteLock();
                }

                if (CollectionLock.IsReadLockHeld)
                {
                    CollectionLock.ExitReadLock();
                }
            }
        }

        protected override void InsertItem(int index, TItem item)
        {
            CollectionLock.EnterUpgradeableReadLock();

            try
            {
                CollectionLock.EnterWriteLock();

                base.InsertItem(index, item);

                CollectionLock.ExitWriteLock();

                this.RaiseCollectionChanged(NotifyCollectionChangedAction.Add, (object)item, index);
            }
            finally
            {
                if (CollectionLock.IsUpgradeableReadLockHeld)
                {
                    CollectionLock.ExitUpgradeableReadLock();
                }

                if (CollectionLock.IsWriteLockHeld)
                {
                    CollectionLock.ExitWriteLock();
                }

                if (CollectionLock.IsReadLockHeld)
                {
                    CollectionLock.ExitReadLock();
                }
            }
        }

        protected override void SetItem(int index, TItem item)
        {
            CollectionLock.EnterUpgradeableReadLock();

            try
            {
                CollectionLock.EnterWriteLock();

                TItem obj = this[index];
                base.SetItem(index, item);

                CollectionLock.ExitWriteLock();

                this.RaiseCollectionChanged(NotifyCollectionChangedAction.Replace, (object)item, (object)obj, index);
            }
            finally
            {
                if (CollectionLock.IsUpgradeableReadLockHeld)
                {
                    CollectionLock.ExitUpgradeableReadLock();
                }

                if (CollectionLock.IsWriteLockHeld)
                {
                    CollectionLock.ExitWriteLock();
                }

                if (CollectionLock.IsReadLockHeld)
                {
                    CollectionLock.ExitReadLock();
                }
            }
        }

        protected override void ClearItems()
        {
            CollectionLock.EnterUpgradeableReadLock();

            try
            {
                CollectionLock.EnterWriteLock();

                base.ClearItems();

                CollectionLock.ExitWriteLock();

                RaiseCollectionReset();
            }
            finally
            {
                if (CollectionLock.IsUpgradeableReadLockHeld)
                {
                    CollectionLock.ExitUpgradeableReadLock();
                }

                if (CollectionLock.IsWriteLockHeld)
                {
                    CollectionLock.ExitWriteLock();
                }

                if (CollectionLock.IsReadLockHeld)
                {
                    CollectionLock.ExitReadLock();
                }
            }
        }
    }
}
