using SquaredInfinity.Foundation.ContextDataCollectors;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Diagnostics.ContextDataCollectors
{
    public class ContextDataCollectorCollection : Collection<IContextDataCollector>
    {
        internal ReadOnlyCollection<IContextDataCollector> SyncCollectors =
            new ReadOnlyCollection<IContextDataCollector>(new List<IContextDataCollector>());

        internal ReadOnlyCollection<IContextDataCollector> AsyncCollectors =
            new ReadOnlyCollection<IContextDataCollector>(new List<IContextDataCollector>());

        readonly ReaderWriterLockSlim CacheLock = new ReaderWriterLockSlim();

        public IDiagnosticEventPropertyCollection Collect()
        {
            var result = new DiagnosticEventPropertyCollection();

            for (int i = 0; i < SyncCollectors.Count; i++)
            {
                var c = SyncCollectors[i];
           //     result.AddOrUpdateFrom(c.CollectData());
            }

            if (AsyncCollectors.Count > 0)
            {
                Parallel.ForEach(AsyncCollectors, (c) =>
                {
             //       result.AddOrUpdateFrom(c.CollectData());
                });
            }

            return result;
        }

        public IReadOnlyList<DiagnosticEventProperty> Collect(IList<DataRequest> requestedContextData)
        {
            var result = new List<DiagnosticEventProperty>();

            for (int i = 0; i < SyncCollectors.Count; i++)
            {
                var c = SyncCollectors[i];
          //      result.AddRange(c.CollectData(requestedContextData));
            }

            // TODO: Async collectors not supported at the moment
            // will need to collect in concurrent bag first (< 4.5 support needed)

            //if (AsyncCollectors.Count > 0)
            //{
            //    Parallel.ForEach(AsyncCollectors, (c) =>
            //    {
            //        result.AddRange(c.CollectData(requestedContextData));
            //    });
            //}

            return result;
        }

        void RefreshCache()
        {
            CacheLock.EnterWriteLock();

            try
            {
                SyncCollectors =
                    new ReadOnlyCollection<IContextDataCollector>(
                        (from s in this
                         where !s.IsAsync
                         select s)
                         .ToList());

                AsyncCollectors =
                    new ReadOnlyCollection<IContextDataCollector>(
                        (from s in this
                         where s.IsAsync
                         select s)
                        .ToList());
            }
            finally
            {
                CacheLock.ExitWriteLock();
            }
        }

        protected override void ClearItems()
        {
            base.ClearItems();
            RefreshCache();
        }

        protected override void InsertItem(int index, IContextDataCollector item)
        {
            base.InsertItem(index, item);
            RefreshCache();
        }

        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);
            RefreshCache();
        }

        protected override void SetItem(int index, IContextDataCollector item)
        {
            base.SetItem(index, item);
            RefreshCache();
        }

        public ContextDataCollectorCollection Clone()
        {
            var result = new ContextDataCollectorCollection();

            foreach (var c in this)
            {
                result.Add(c.Clone());
            }

            return result;
        }
    }
}
