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

        private void RaiseCollectionChanged(NotifyCollectionChangedAction action, object newItem, object oldItem, int index)
        {
            IncrementVersion();

            // Collection Changed events must be raised on UI thread to satisfy WPF requirements
     //       SyncContext.Send((_) =>
        //    {
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

       //     }, null);
        }

        void RaiseCollectionChanged(NotifyCollectionChangedAction action, object item, int index)
        {
            IncrementVersion();

            // Collection Changed events must be raised on UI thread to satisfy WPF requirements
       //     SyncContext.Send((_) =>
       //     {
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

       //     }, null);
        }

        void RaiseCollectionChanged(NotifyCollectionChangedAction action, object item, int index, int oldIndex)
        {
            IncrementVersion();

            // Collection Changed events must be raised on UI thread to satisfy WPF requirements
        //    SyncContext.Send((_) =>
         //   {
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

      //      }, null);
        }

        void RaiseCollectionReset()
        {
            IncrementVersion();

            // Collection Changed events must be raised on UI thread to satisfy WPF requirements
         //   SyncContext.Send((_) =>
        //    {
                if (CollectionChanged != null)
                {
                    CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                }

                RaisePropertyChanged("Count");
                RaisePropertyChanged("Item[]");
                RaisePropertyChanged("Version");

     //       }, null);
        }
    }
}
