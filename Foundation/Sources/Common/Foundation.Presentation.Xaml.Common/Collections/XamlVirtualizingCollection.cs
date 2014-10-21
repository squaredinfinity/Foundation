using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SquaredInfinity.Foundation.Extensions;
using SquaredInfinity.Foundation.Collections;
using System.ComponentModel;
using SquaredInfinity.Foundation.Threading;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Data;
using System.Collections;
using System.Runtime.CompilerServices;

namespace SquaredInfinity.Foundation.Presentation.Collections
{
    public class XamlVirtualizingCollection<TDataItem> :
           ProviderBasedVirtualizingCollection<TDataItem>,
           INotifyCollectionChanged,
           INotifyPropertyChanged
           where TDataItem : class
    {
        readonly protected ILock CollectionLock = new ReaderWriterLockSlimEx(LockRecursionPolicy.SupportsRecursion);
        readonly Dispatcher Dispatcher;

        static Dispatcher GetMainThreadDispatcher()
        {
            if (Application.Current != null && Application.Current.Dispatcher != null)
                return Application.Current.Dispatcher;

            return Dispatcher.CurrentDispatcher;
        }

        public XamlVirtualizingCollection(IVirtualizedDataItemsProvider<TDataItem> itemsProvider)
            : base(itemsProvider)
        {
            Dispatcher = GetMainThreadDispatcher();

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

            using (lockAcquisition)
            {
                accessMethod();
            }
        }

        protected override void HandleUnderlyingDataChange()
        {
            using (CollectionLock.AcquireWriteLock())
            {
                //# remove pages which are no longer used
                CleanUp();

                //# for each item which is still in use,
                //# update it to the current value if needed

                //foreach (var kvp in Pages)
                //{
                //    foreach (var item in kvp.Value.Items)
                //    {
                //        // raise item changed
                //        //item.Index;

                //    }
                //}

                var oldCount = Count;

                Count = -1;

                RaiseCollectionReset();
            }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        protected void RaiseCollectionChanged(NotifyCollectionChangedAction action, object newItem, object oldItem, int index)
        {
            if (action.IsIn(NotifyCollectionChangedAction.Move, NotifyCollectionChangedAction.Replace))
            {
                RaisePropertyChanged("Item[]");
            }
            else
            {
                RaisePropertyChanged("Item[]");
                RaisePropertyChanged("Count");
            }

            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, newItem, oldItem, index));
        }

        protected void RaiseCollectionChanged(NotifyCollectionChangedAction action, object item, int index)
        {
            if (action.IsIn(NotifyCollectionChangedAction.Move, NotifyCollectionChangedAction.Replace))
            {
                RaisePropertyChanged("Item[]");
            }
            else
            {
                RaisePropertyChanged("Item[]");
                RaisePropertyChanged("Count");
            }

            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, item, index));
        }

        protected void RaiseCollectionChanged(NotifyCollectionChangedAction action, object item, int index, int oldIndex)
        {
            if (action.IsIn(NotifyCollectionChangedAction.Move, NotifyCollectionChangedAction.Replace))
            {
                RaisePropertyChanged("Item[]");
            }
            else
            {
                RaisePropertyChanged("Item[]");
                RaisePropertyChanged("Count");
            }

            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, item, index, oldIndex));
        }

        protected void RaiseCollectionReset()
        {
            RaisePropertyChanged("Count");
            RaisePropertyChanged("Item[]");

            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void RaiseThisPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
