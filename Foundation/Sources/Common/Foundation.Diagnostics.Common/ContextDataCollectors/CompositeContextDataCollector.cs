﻿using SquaredInfinity.Foundation.ContextDataCollectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Diagnostics.ContextDataCollectors
{
    public class CompositeContextDataCollector : ContextDataCollector, ICollection<IContextDataCollector>
    {
        ContextDataCollectorCollection _collectors = new ContextDataCollectorCollection();
        ContextDataCollectorCollection Collectors
        {
            get { return _collectors; }
            set { _collectors = value; }
        }

        public override
#if UPTO_DOTNET40 
        IList<DiagnosticEventProperty>
#else
 IReadOnlyList<DiagnosticEventProperty>
#endif
 CollectData()
        {
            var result = Collectors.Collect().ToList();

            return result;
        }

        public override bool TryGetData(DataRequest dataRequest, DataCollectionContext context, out object result)
        {
            for (int i = 0; i < Collectors.Count; i++)
            {
                var collector = Collectors[i];

                if (collector.TryGetData(dataRequest, context, out result))
                    return true;
            }

            result = null;
            return false;
        }

        public void Add(IContextDataCollector item)
        {
            Collectors.Add(item);
        }

        public void Clear()
        {
            Collectors.Clear();
        }

        public bool Contains(IContextDataCollector item)
        {
            return Collectors.Contains(item);
        }

        public void CopyTo(IContextDataCollector[] array, int arrayIndex)
        {
            Collectors.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return Collectors.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(IContextDataCollector item)
        {
            return Collectors.Remove(item);
        }

        public IEnumerator<IContextDataCollector> GetEnumerator()
        {
            return Collectors.AsEnumerable().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Collectors.GetEnumerator();
        }

        public override IContextDataCollector Clone()
        {
            var result = new CompositeContextDataCollector();

            result.IsAsync = IsAsync;

            foreach (var c in Collectors)
                result.Collectors.Add(c.Clone());

            return result;
        }
    }
}
