using SquaredInfinity.Foundation.Collections;
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
    public class ContextDataCollectorCollection : CollectionEx<IContextDataCollector>, IContextDataCollectorCollection
    {
        internal ReadOnlyCollection<IContextDataCollector> SyncCollectors =
            new ReadOnlyCollection<IContextDataCollector>(new List<IContextDataCollector>());

        internal ReadOnlyCollection<IContextDataCollector> AsyncCollectors =
            new ReadOnlyCollection<IContextDataCollector>(new List<IContextDataCollector>());

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

        public IDiagnosticEventPropertyCollection Collect(IReadOnlyList<IDataRequest> requestedContextData)
        {
            var result = new DiagnosticEventPropertyCollection(capacity: requestedContextData.Count);

            for (int i = 0; i < SyncCollectors.Count; i++)
            {
                var c = SyncCollectors[i];
                result.AddOrUpdateRange(c.CollectData(requestedContextData));
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

        protected override void OnAfterVersionChanged(long newVersion)
        {
            base.OnAfterVersionChanged(newVersion);

            RefreshCache();
        }

        void RefreshCache()
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
    }
}
