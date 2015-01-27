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
    public partial class XamlObservableCollectionEx<TItem> : 
        ObservableCollectionEx<TItem>,
        System.Collections.IList
    {
        readonly Dispatcher Dispatcher;

        object IList.this[int index] { get { return this[index]; } set { this[index] = (TItem)value; } }

        static Dispatcher GetMainThreadDispatcher()
        {
            if(Application.Current != null && Application.Current.Dispatcher != null)
                return Application.Current.Dispatcher;

            return Dispatcher.CurrentDispatcher;
        }
        
        public XamlObservableCollectionEx()
            : this(21, GetMainThreadDispatcher(), monitorElementsForChanges:false)
        { }

        public XamlObservableCollectionEx(bool monitorElementsForChanges)
            : this(21, GetMainThreadDispatcher(), monitorElementsForChanges)
        { }

        public XamlObservableCollectionEx(
            int capacity, 
            Dispatcher dispatcher,
            bool monitorElementsForChanges)
            : base(monitorElementsForChanges, new List<TItem>(capacity))
        {
            Dispatcher = dispatcher;
            
            //NOTE: BindingOperations.EnableCollectionSynchronization is not needed when SynchornizationContext.Current is null (e.g. running as windows service or 
            //TODO: it may also be null in unit tests though :|
            //if(System.Threading.SynchronizationContext.Current != null)

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
    }
}
