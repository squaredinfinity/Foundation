using SquaredInfinity.Foundation.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;

namespace SquaredInfinity.Foundation.Collections
{
    public partial class ObservableCollectionEx<TItem> : Collection<TItem>
    {
        readonly ILock CollectionLock = new ReaderWriterLockSlimEx();

        readonly Dispatcher Dispatcher;

        static Dispatcher GetMainThreadDispatcher()
        {
            if(Application.Current != null && Application.Current.Dispatcher != null)
                return Application.Current.Dispatcher;

            return Dispatcher.CurrentDispatcher;
        }

        public ObservableCollectionEx()
            : this(21, GetMainThreadDispatcher())
        { }

        public ObservableCollectionEx(
            int capacity, 
            Dispatcher dispatcher)
            : base(new List<TItem>(capacity))
        {
            Dispatcher = dispatcher;

            BindingOperations.EnableCollectionSynchronization(this, context: null, synchronizationCallback: BindingSync);
        }

        void BindingSync(IEnumerable collection, object context, Action accessMethod, bool writeAccess)
        {
            var lockAcquisition = (IDisposable)null;

            if (writeAccess)
            {
                lockAcquisition = CollectionLock.AcquireReadLock();
            }
            else
            {
                lockAcquisition = CollectionLock.AcquireWriteLock();
            }

            using(lockAcquisition)
            { 
                accessMethod();
            }
        }

        public void Move(int oldIndex, int newIndex)
        {
            using(var readLock = CollectionLock.AcquireUpgradeableReadLock())
            {
                var obj = default(TItem);

                using(readLock.AcquireWriteLock())
                {
                    obj = this[oldIndex];
                    base.RemoveItem(oldIndex);
                    base.InsertItem(newIndex, obj);
                }

                RaiseCollectionChanged(NotifyCollectionChangedAction.Move, (object)obj, newIndex, oldIndex);
            }
        }

        protected override void RemoveItem(int index)
        {
            using(var readLock = CollectionLock.AcquireUpgradeableReadLock())
            {
                var obj = default(TItem);

                using(readLock.AcquireWriteLock())
                {
                    obj = this[index];
                    base.RemoveItem(index);
                }

                this.RaiseCollectionChanged(NotifyCollectionChangedAction.Remove, (object)obj, index);
            }
        }

        protected override void InsertItem(int index, TItem item)
        {
            using(var readLock = CollectionLock.AcquireUpgradeableReadLock())
            { 
                using(readLock.AcquireWriteLock())
                {
                    base.InsertItem(index, item);
                }

                this.RaiseCollectionChanged(NotifyCollectionChangedAction.Add, (object)item, index);
            }
        }

        protected override void SetItem(int index, TItem item)
        {
            using(var readLock = CollectionLock.AcquireUpgradeableReadLock())
            {
                var obj = default(TItem);

                using(CollectionLock.AcquireWriteLock())
                {
                    obj = this[index];
                    base.SetItem(index, item);
                }

                this.RaiseCollectionChanged(NotifyCollectionChangedAction.Replace, (object)item, (object)obj, index);
            }
        }

        protected override void ClearItems()
        {
            using(var readLock = CollectionLock.AcquireUpgradeableReadLock())
            { 
                using(CollectionLock.AcquireWriteLock())
                {
                    base.ClearItems();
                }

                RaiseCollectionReset();
            }
        }

        public Task AddRangeAsync(IEnumerable<TItem> items, CancellationToken cancellationToken)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }
            else
            {
                List<TItem> list = new List<TItem>(items);

                return Task.Factory.StartNew(() =>
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            if (cancellationToken.IsCancellationRequested)
                                return;

                            try
                            {
                                Dispatcher.Invoke(() =>
                                    {
                                        using (var writeLock = CollectionLock.AcquireWriteLock())
                                        {
                                            var item = list[i];

                                            if (cancellationToken.IsCancellationRequested)
                                                return;

                                            Items.Add(item);

                                            RaiseCollectionChanged(NotifyCollectionChangedAction.Add, (object)item, Items.Count - 1);
                                        }
                                    }, DispatcherPriority.Background, cancellationToken);
                            }
                            catch (OperationCanceledException)
                            {
                                // operation has been cancelled, nothing more needs to be done
                            }
                        }
                    }, cancellationToken);
            }
        }
    }
}
