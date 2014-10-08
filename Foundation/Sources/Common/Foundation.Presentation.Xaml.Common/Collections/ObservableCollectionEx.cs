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
using SquaredInfinity.Foundation.Extensions;

namespace SquaredInfinity.Foundation.Collections
{
    public class AfterItemAddedEventArgs<TItem> : EventArgs
    {
        public TItem AddedItem { get; private set; }

        public AfterItemAddedEventArgs(TItem addedItem)
        {
            this.AddedItem = addedItem;
        }
    }

    public class AfterItemRemovedEventArgs<TItem> : EventArgs
    {
        public TItem RemovedItem { get; private set; }

        public AfterItemRemovedEventArgs(TItem removedItem)
        {
            this.RemovedItem = removedItem;
        }
    }

    public partial class ObservableCollectionEx<TItem> : 
        Collection<TItem>, 
        ICollectionEx<TItem>,
        System.Collections.IList
    {
        readonly protected ILock CollectionLock = new ReaderWriterLockSlimEx(LockRecursionPolicy.SupportsRecursion);

        readonly Dispatcher Dispatcher;

        int InitialHashCode;

        object IList.this[int index] { get { return this[index]; } set { this[index] = (TItem)value; } }

        static Dispatcher GetMainThreadDispatcher()
        {
            if(Application.Current != null && Application.Current.Dispatcher != null)
                return Application.Current.Dispatcher;

            return Dispatcher.CurrentDispatcher;
        }

        bool MonitorElementsForChanges { get; set; }

        public ObservableCollectionEx()
            : this(21, GetMainThreadDispatcher(), monitorElementsForChanges:false)
        { }

        public ObservableCollectionEx(bool monitorElementsForChanges)
            : this(21, GetMainThreadDispatcher(), monitorElementsForChanges)
        { }

        public ObservableCollectionEx(
            int capacity, 
            Dispatcher dispatcher,
            bool monitorElementsForChanges)
            : base(new List<TItem>(capacity))
        {
            Dispatcher = dispatcher;

            if (monitorElementsForChanges == true 
                && 
                (typeof(TItem).ImplementsOrExtends(typeof(INotifyPropertyChanged)) || typeof(TItem).ImplementsOrExtends(typeof(INotifyVersionChangedObject)))
               )
            {
                MonitorElementsForChanges = monitorElementsForChanges;
            }

            InitialHashCode = GetHashCode();

            BindingOperations.EnableCollectionSynchronization(this, context: null, synchronizationCallback: BindingSync);
        }

        void BindingSync(IEnumerable collection, object context, Action accessMethod, bool writeAccess)
        {
            var lockAcquisition = (IDisposable)null;

            if (writeAccess)
            {
                lockAcquisition = CollectionLock.AcquireWriteLock();
            }
            else
            {
                lockAcquisition = CollectionLock.AcquireReadLock();
            }

            using(lockAcquisition)
            { 
                accessMethod();
            }
        }

        public virtual void Replace(TItem oldItem, TItem newItem)
        {
            using (CollectionLock.AcquireUpgradeableReadLock())
            {
                var oldIndex = IndexOf(oldItem);

                if (oldIndex < 0)
                    return;

                SetItem(oldIndex, newItem);
            }
        }

        public void Move(int oldIndex, int newIndex)
        {
            if (oldIndex == newIndex)
                return;

            using(var readLock = CollectionLock.AcquireUpgradeableReadLock())
            {
                var obj = default(TItem);

                using(readLock.AcquireWriteLock())
                {
                    obj = this[oldIndex];
                    base.RemoveItem(oldIndex);
                    base.InsertItem(newIndex, obj);
                }

                // NOTE:    Action.Move seems to not work properly with CollectionViewSources (UI does not update correctly in some cases, when custom sorting/filtering is applied)
                //          Use Remove + Add instead
                //          Raise Version Changed only once
                
                //x RaiseCollectionChanged(NotifyCollectionChangedAction.Move, (object)obj, newIndex, oldIndex);

                RaiseCollectionChanged(NotifyCollectionChangedAction.Remove, (object)obj, oldIndex, raiseVersionChanged: false);
                RaiseCollectionChanged(NotifyCollectionChangedAction.Add, (object)obj, newIndex, raiseVersionChanged: false);

                RaisePropertyChanged("Version");
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

                    if (MonitorElementsForChanges)
                    {
                        StopItemChangeMonitoring(obj);
                    }

                    OnItemRemoved(obj);
                }

                this.RaiseCollectionChanged(NotifyCollectionChangedAction.Remove, (object)obj, index);
            }
        }

        public event EventHandler<AfterItemRemovedEventArgs<TItem>> AfterItemRemoved;

        void OnItemRemoved(TItem item)
        {
            if (AfterItemRemoved != null)
                AfterItemRemoved(this, new AfterItemRemovedEventArgs<TItem>(item));
        }


        public event EventHandler<AfterItemAddedEventArgs<TItem>> AfterItemAdded;

        protected override void InsertItem(int index, TItem item)
        {
            using(var readLock = CollectionLock.AcquireUpgradeableReadLock())
            { 
                using(readLock.AcquireWriteLock())
                {
                    base.InsertItem(index, item);

                    if (MonitorElementsForChanges)
                        BeginItemChangeMonitoring(item);
                }

                this.RaiseCollectionChanged(NotifyCollectionChangedAction.Add, (object)item, index);
            }
        }

        void OnAfterItemAdded(TItem item)
        {
            if (AfterItemAdded != null)
                AfterItemAdded(this, new AfterItemAddedEventArgs<TItem>(item));
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

                    if (MonitorElementsForChanges)
                        StopItemChangeMonitoring(obj);
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

                            var op = Dispatcher.BeginInvoke(new Action(() =>
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
                                            BeginItemChangeMonitoring(item);
                                        }
                                    }
                                }), DispatcherPriority.Background);

                            
                            op.Task.Wait(ignoreCanceledExceptions:true);
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

        void HandleItemVersionChanged(object sender, EventArgs e)
        {
            // todo:    in future this may need to raise an event with exact item that changed communicated to subscribers,
            //          but there was not need for it so far.

            this.IncrementVersion();
        }

        void BeginItemChangeMonitoring(object item)
        {
            var vco = item as INotifyVersionChangedObject;
            if(vco != null)
            {
                vco.VersionChanged += HandleItemVersionChanged;

                // version changed already includes property changes, so just return
                return;
            }

            var inpc = item as INotifyPropertyChanged;
            if (inpc != null)
                inpc.PropertyChanged += HandleItemPropertyChanged;
        }

        void StopItemChangeMonitoring(object item)
        {
            var vco = item as INotifyVersionChangedObject;
            if (vco != null)
                vco.VersionChanged -= HandleItemVersionChanged;

            var inpc = item as INotifyPropertyChanged;
            if (inpc != null)
                inpc.PropertyChanged -= HandleItemPropertyChanged;
        }


        public IReadOnlyList<TItem> GetSnapshot()
        {
            using(CollectionLock.AcquireReadLock())
            {
                var snapshot = this.ToArray();
                
                return snapshot;
            }
        }
    }
}
