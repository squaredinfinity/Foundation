using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SquaredInfinity.Foundation.Extensions;

namespace SquaredInfinity.Foundation.Collections
{
    public partial class ObservableCollectionEx<TItem> : CollectionEx<TItem>
    {
        readonly int InitialHashCode;

        public ObservableCollectionEx()
            : this(monitorElementsForChanges: false, items: new List<TItem>())
        { }

        public ObservableCollectionEx(bool monitorElementsForChanges, IList<TItem> items)
            : base(items)
        {
            InitialHashCode = GetHashCode();

            if (monitorElementsForChanges == true
                &&
                (typeof(TItem).ImplementsOrExtends(typeof(INotifyPropertyChanged)) || typeof(TItem).ImplementsOrExtends(typeof(INotifyVersionChangedObject)))
                )
            {
                MonitorElementsForChanges = monitorElementsForChanges;
            }
        }

        #region Move

        protected override void OnAfterItemMoved(TItem item, int oldIndex, int newIndex)
        {
            // NOTE:    Action.Move seems to not work properly with CollectionViewSources (UI does not update correctly in some cases, when custom sorting/filtering is applied)
            //          Use Remove + Add instead
            //          Raise Version Changed only once

            //x RaiseCollectionChanged(NotifyCollectionChangedAction.Move, (object)obj, newIndex, oldIndex);

            RaiseCollectionChanged(NotifyCollectionChangedAction.Remove, (object)item, oldIndex);
            RaiseCollectionChanged(NotifyCollectionChangedAction.Add, (object)item, newIndex);
        }

        #endregion

        #region Clear

        protected override void OnBeforeItemsCleared(IReadOnlyList<TItem> oldItems)
        {
            if (MonitorElementsForChanges)
            {
                foreach (var item in oldItems)
                {
                    (item as INotifyPropertyChanged).PropertyChanged -= HandleItemPropertyChanged;
                }
            }
        }

        protected override void OnAfterItemsCleared(IReadOnlyList<TItem> oldItems)
        {
            for (int i = 0; i < oldItems.Count; i++)
            {
                RaiseAfterItemRemoved(oldItems[i]);
            }

            RaiseCollectionReset();
        }

        #endregion

        #region Insert / Add

        protected override void OnBeforeItemInserted(int index, TItem item)
        {
            if (MonitorElementsForChanges)
                BeginItemChangeMonitoring(item);
        }

        protected override void OnAfterItemInserted(int index, TItem item)
        {
            RaiseCollectionChanged(NotifyCollectionChangedAction.Add, (object) item, index);
            RaiseAfterItemAdded(item);
        }

        public event EventHandler<AfterItemAddedEventArgs<TItem>> AfterItemAdded;
        void RaiseAfterItemAdded(TItem item)
        {
            if (AfterItemAdded != null)
                AfterItemAdded(this, new AfterItemAddedEventArgs<TItem>(item));
        }

        #endregion

        #region Remove

        protected override void OnBeforeItemRemoved(TItem item, int index)
        {
            if (MonitorElementsForChanges)
            {
                StopItemChangeMonitoring(item);
            }
        }

        protected override void OnAfterItemRemoved(TItem item, int index)
        {
            RaiseCollectionChanged(NotifyCollectionChangedAction.Remove, (object)item, index);
            RaiseAfterItemRemoved(item);
        }

        public event EventHandler<AfterItemRemovedEventArgs<TItem>> AfterItemRemoved;
        void RaiseAfterItemRemoved(TItem item)
        {
            if (AfterItemRemoved != null)
                AfterItemRemoved(this, new AfterItemRemovedEventArgs<TItem>(item));
        }

        #endregion

        #region Replace / Set

        protected override void OnBeforeItemReplaced(int index, TItem oldItem, TItem newItem)
        {
            if (MonitorElementsForChanges)
                StopItemChangeMonitoring(oldItem);
        }

        protected override void OnAfterItemReplaced(int index, TItem oldItem, TItem newItem)
        {
            RaiseCollectionChanged(NotifyCollectionChangedAction.Replace, (object)newItem, (object)oldItem, index);
            RaiseAfterItemRemoved(oldItem);
            RaiseAfterItemAdded(newItem);
        }

        #endregion

        #region Items Monitoring

        protected bool MonitorElementsForChanges { get; private set; }

        void HandleItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // todo:    in future this may need to raise an event with exact item that changed communicated to subscribers,
            //          but there was not need for it so far.

            base.IncrementVersion();
        }

        void HandleItemVersionChanged(object sender, EventArgs e)
        {
            // todo:    in future this may need to raise an event with exact item that changed communicated to subscribers,
            //          but there was not need for it so far.

            base.IncrementVersion();
        }

        protected void BeginItemChangeMonitoring(object item)
        {
            var vco = item as INotifyVersionChangedObject;
            if (vco != null)
            {
                vco.VersionChanged -= HandleItemVersionChanged;
                vco.VersionChanged += HandleItemVersionChanged;

                // version changed already includes property changes, so just return
                return;
            }

            var inpc = item as INotifyPropertyChanged;
            if (inpc != null)
            {
                inpc.PropertyChanged -= HandleItemPropertyChanged;
                inpc.PropertyChanged += HandleItemPropertyChanged;
            }
        }

        protected void StopItemChangeMonitoring(object item)
        {
            var vco = item as INotifyVersionChangedObject;
            if (vco != null)
                vco.VersionChanged -= HandleItemVersionChanged;

            var inpc = item as INotifyPropertyChanged;
            if (inpc != null)
                inpc.PropertyChanged -= HandleItemPropertyChanged;
        }

        #endregion

        #region Sort

        public void Sort(IComparer<TItem> comparer)
        {
            using (CollectionLock.AcquireWriteLock())
            {
                var copy = GetSnapshot();

                (base.Items as List<TItem>).Sort(comparer);

                for (int i = 0; i < copy.Count; i++)
                {
                    var ix_new = Items.IndexOf(copy[i]);

                    if (ix_new != i)
                    {
                        // NOTE:    Action.Move seems to not work properly with CollectionViewSources (UI does not update correctly in some cases, when custom sorting/filtering is applied)
                        //          Use Remove + Add instead
                        //          Raise Version Changed only once

                        //x RaiseCollectionChanged(NotifyCollectionChangedAction.Move, (object)obj, newIndex, oldIndex);

                        RaiseCollectionChanged(NotifyCollectionChangedAction.Remove, (object)copy[i], i);
                        RaiseCollectionChanged(NotifyCollectionChangedAction.Add, (object)copy[i], ix_new);
                    }
                }
            }
        }

        #endregion

        #region Snapshot

        public IReadOnlyList<TItem> GetSnapshot()
        {
            using (CollectionLock.AcquireReadLock())
            {
                var snapshot = this.ToArray();

                return snapshot;
            }
        }

        #endregion

        protected override void IncrementVersion()
        {
            base.IncrementVersion();

            RaisePropertyChanged(() => Version);
        }
    }
}
