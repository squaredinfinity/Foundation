using SquaredInfinity.Foundation.Diagnostics.Filters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SquaredInfinity.Foundation.Extensions;
using System.Collections.Concurrent;

namespace SquaredInfinity.Foundation.Diagnostics.ContextDataCollectors
{
    public abstract class ContextDataCollector : IContextDataCollector
    {
        bool _isAsync = false;
        public bool IsAsync
        {
            get { return _isAsync; }
            set { _isAsync = value; }
        }

        string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        IFilter _filter;
        public IFilter Filter
        {
            get { return _filter; }
            set { _filter = value; }
        }

        readonly IDataRequestCollection _requestedData = new DataRequestCollection();
        public IDataRequestCollection RequestedData
        {
            get { return _requestedData; }
        }

        readonly ConcurrentDictionary<IDataRequest, IDiagnosticEventProperty> _dataRequestCache
            = new ConcurrentDictionary<IDataRequest, IDiagnosticEventProperty>();
        protected ConcurrentDictionary<IDataRequest, IDiagnosticEventProperty> DataRequestCache
        {
            get { return _dataRequestCache; }
        }

        public abstract bool TryGetData(IDataRequest dataRequest, IDataCollectionContext context, out object result);

        public virtual IDiagnosticEventPropertyCollection CollectData()
        {
            return CollectData(RequestedData);
        }

        public IDiagnosticEventPropertyCollection CollectData(IReadOnlyList<IDataRequest> requestedContextData)
        {
            var result = new DiagnosticEventPropertyCollection(capacity: requestedContextData.Count);

            if (requestedContextData.Count == 0)
                return result;

            var cx = new DataCollectionContext();

            for (int i = 0; i < requestedContextData.Count; i++)
            {
                var item = requestedContextData[i];

                IDiagnosticEventProperty property = null;

                //# try to get from cache first
                if (DataRequestCache.TryGetValue(item, out property))
                {
                    result.AddOrUpdate(property);
                    continue;
                }

                //# get the data
                var dataValue = (object)null;

                if (TryGetData(item, cx, out dataValue))
                {
                    var newProperty = new DiagnosticEventProperty();
                    newProperty.UniqueName = item.Data;
                    newProperty.Value = dataValue;

                    property = newProperty;

                    result.AddOrUpdate(property);

                    // add to cache if needed
                    if (item.IsCached && !DataRequestCache.ContainsKey(item))
                        DataRequestCache.AddOrUpdate(item, property);
                }
            }

            return result;
        }
    }
}
