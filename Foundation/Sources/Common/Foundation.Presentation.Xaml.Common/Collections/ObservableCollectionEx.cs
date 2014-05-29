using SquaredInfinity.Foundation.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
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

        bool MonitorElementsForChanges { get; set; }

        public ObservableCollectionEx()
            : this(21, GetMainThreadDispatcher(), monitorElementsForChanges:true)
        { }

        public ObservableCollectionEx(
            int capacity, 
            Dispatcher dispatcher,
            bool monitorElementsForChanges)
            : base(new List<TItem>(capacity))
        {
            Dispatcher = dispatcher;

            if (monitorElementsForChanges == true && typeof(TItem).IsAssignableFrom(typeof(INotifyPropertyChanged)))
            {
                MonitorElementsForChanges = monitorElementsForChanges;
            }

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

                    if(MonitorElementsForChanges)
                        (obj as INotifyPropertyChanged).PropertyChanged -= HandleItemPropertyChanged;
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

                    if(MonitorElementsForChanges)
                        (item as INotifyPropertyChanged).PropertyChanged += HandleItemPropertyChanged;
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

                    if(MonitorElementsForChanges)
                        (obj as INotifyPropertyChanged).PropertyChanged -= HandleItemPropertyChanged;
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
                    if(MonitorElementsForChanges)
                    {
                        foreach(var item in Items)
                        {
                            (item as INotifyPropertyChanged).PropertyChanged -= HandleItemPropertyChanged;
                        }
                    }

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

                                            if (MonitorElementsForChanges)
                                            {
                                                (item as INotifyPropertyChanged).PropertyChanged += HandleItemPropertyChanged;
                                            }
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

        void HandleItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // todo:    in future this may need to raise an event with exact item that changed communicated to subscribers,
            //          but there was not need for it so far.

            this.IncrementVersion();
        }
    }
}
