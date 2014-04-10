using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using SquaredInfinity.Foundation.Extensions;

namespace SquaredInfinity.Foundation.Collections
{
    public partial class ObservableCollectionEx<TItem> :  INotifyCollectionChanged
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        void RaiseCollectionChanged(NotifyCollectionChangedAction action, object newItem, object oldItem, int index)
        {
            IncrementVersion();

            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, newItem, oldItem, index));

            if (action.IsIn(NotifyCollectionChangedAction.Move, NotifyCollectionChangedAction.Replace))
            {
                RaisePropertyChanged("Item[]");
            }
            else
            {
                RaisePropertyChanged("Item[]");
                RaisePropertyChanged("Count");
            }

            RaisePropertyChanged("Version");
        }

        void RaiseCollectionChanged(NotifyCollectionChangedAction action, object item, int index)
        {
            IncrementVersion();

            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, item, index));

            if (action.IsIn(NotifyCollectionChangedAction.Move, NotifyCollectionChangedAction.Replace))
            {
                RaisePropertyChanged("Item[]");
            }
            else
            {
                RaisePropertyChanged("Item[]");
                RaisePropertyChanged("Count");
            }

            RaisePropertyChanged("Version");
        }

        void RaiseCollectionChanged(NotifyCollectionChangedAction action, object item, int index, int oldIndex)
        {
            IncrementVersion();

            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, item, index, oldIndex));

            if (action.IsIn(NotifyCollectionChangedAction.Move, NotifyCollectionChangedAction.Replace))
            {
                RaisePropertyChanged("Item[]");
            }
            else
            {
                RaisePropertyChanged("Item[]");
                RaisePropertyChanged("Count");
            }

            RaisePropertyChanged("Version");
        }

        void RaiseCollectionReset()
        {
            IncrementVersion();

            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }

            RaisePropertyChanged("Count");
            RaisePropertyChanged("Item[]");
            RaisePropertyChanged("Version");
        }
    }
}
