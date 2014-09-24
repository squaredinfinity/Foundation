using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using SquaredInfinity.Foundation.Extensions;
using System.Windows.Data;

namespace SquaredInfinity.Foundation.Collections
{
    public partial class ObservableCollectionEx<TItem> :  INotifyCollectionChanged
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        protected void RaiseCollectionChanged(NotifyCollectionChangedAction action, object newItem, object oldItem, int index)
        {
            IncrementVersion();

            if (action.IsIn(NotifyCollectionChangedAction.Move, NotifyCollectionChangedAction.Replace))
            {
                RaisePropertyChanged("Item[]");
            }
            else
            {
                RaisePropertyChanged("Item[]");
                RaisePropertyChanged("Count");
            }

            TryRaiseCollectionChanged(new NotifyCollectionChangedEventArgs(action, newItem, oldItem, index));

            RaisePropertyChanged("Version");
        }

        protected void RaiseCollectionChanged(NotifyCollectionChangedAction action, object item, int index, bool raiseVersionChanged = true)
        {
            IncrementVersion();

            if (action.IsIn(NotifyCollectionChangedAction.Move, NotifyCollectionChangedAction.Replace))
            {
                RaisePropertyChanged("Item[]");
            }
            else
            {
                RaisePropertyChanged("Item[]");
                RaisePropertyChanged("Count");
            }

            TryRaiseCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index));

            if(raiseVersionChanged)
                RaisePropertyChanged("Version");
        }

        protected void RaiseCollectionChanged(NotifyCollectionChangedAction action, object item, int index, int oldIndex)
        {
            IncrementVersion();

            if (action.IsIn(NotifyCollectionChangedAction.Move, NotifyCollectionChangedAction.Replace))
            {
                RaisePropertyChanged("Item[]");
            }
            else
            {
                RaisePropertyChanged("Item[]");
                RaisePropertyChanged("Count");
            }

            TryRaiseCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index, oldIndex));

            RaisePropertyChanged("Version");
        }

        protected void RaiseCollectionReset()
        {
            IncrementVersion();

            TryRaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

            RaisePropertyChanged("Count");
            RaisePropertyChanged("Item[]");
            RaisePropertyChanged("Version");
        }

        void TryRaiseCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            if (CollectionChanged == null)
                return;

            try
            {
                CollectionChanged(this, args);
            }
            catch(NotSupportedException ex)
            {
                if(InitialHashCode != GetHashCode())
                {
                    //! Not Supported Exception can still sometimes be thrown by CollectionView
                    //  even when BindingOperations.EnableCollectionSynchronization() has been used
                    //
                    //  one scenario that I am aware of is when source collection overrides GetHashCode() method
                    //  in such way that hash code is not immutable anymore (e.g. it may change when items in collection change)
                    //  this will cause Binding Engine to lose track of the collection (it assumes hash code to be immutable)

                    var _ex = new NotSupportedException(
                        "GetHashCode() method on type {0} should return immutable value for it to work with XAML."
                        .FormatWith(GetType().FullName));

                    throw _ex;
                }

                throw;
            }
        }
    }
}
