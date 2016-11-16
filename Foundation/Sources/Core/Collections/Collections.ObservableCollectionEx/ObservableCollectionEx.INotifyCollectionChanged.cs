using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;

namespace SquaredInfinity.Foundation.Collections
{
    public partial class ObservableCollectionEx<TItem> :  INotifyCollectionChanged
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        protected void RaiseCollectionChanged(NotifyCollectionChangedAction action, object newItem, object oldItem, int index)
        {
            if (State == STATE__BULKUPDATE)
                return;

            if (action.IsIn(NotifyCollectionChangedAction.Move, NotifyCollectionChangedAction.Replace))
            {
                RaiseIndexerChanged();
            }
            else
            {
                RaiseIndexerChanged();
                RaisePropertyChanged(nameof(Count));
            }

            TryRaiseCollectionChanged(new NotifyCollectionChangedEventArgs(action, newItem, oldItem, index));
        }

        protected void RaiseCollectionChanged(NotifyCollectionChangedAction action, object item, int index)
        {
            if (State == STATE__BULKUPDATE)
                return;

            if (action.IsIn(NotifyCollectionChangedAction.Move, NotifyCollectionChangedAction.Replace))
            {
                RaiseIndexerChanged();
            }
            else
            {
                RaiseIndexerChanged();
                RaisePropertyChanged(nameof(Count));
            }

            TryRaiseCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index));
        }

        protected void RaiseCollectionChanged(NotifyCollectionChangedAction action, object item, int index, int oldIndex)
        {
            if (State == STATE__BULKUPDATE)
                return;

            if (action.IsIn(NotifyCollectionChangedAction.Move, NotifyCollectionChangedAction.Replace))
            {
                RaiseIndexerChanged();
            }
            else
            {
                RaiseIndexerChanged();
                RaisePropertyChanged(nameof(Count));
            }

            TryRaiseCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index, oldIndex));
        }

        protected void RaiseCollectionReset()
        {
            if (State == STATE__BULKUPDATE)
                return;
            
            TryRaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

            RaiseIndexerChanged();
            RaisePropertyChanged(nameof(Count));
        }

        void TryRaiseCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            if (CollectionChanged == null)
                return;

            try
            {
                CollectionChanged(this, args);
            }
            catch (NotSupportedException ex)
            {
                if (InitialHashCode != GetHashCode())
                {
                    //! Not Supported Exception can still sometimes be thrown by CollectionView
                    //  even when BindingOperations.EnableCollectionSynchronization() has been used
                    //
                    //  one scenario that I am aware of is when source collection overrides GetHashCode() method
                    //  in such way that hash code is not immutable anymore (e.g. it may change when items in collection change)
                    //  this will cause Binding Engine to lose track of the collection (it assumes hash code to be immutable)

                    var _ex = new NotSupportedException(
                        $"GetHashCode() method on type {GetType().FullName} should return immutable value for it to work with XAML.", 
                        ex);

                    throw _ex;
                }

                throw;
            }
        }
    }
}
