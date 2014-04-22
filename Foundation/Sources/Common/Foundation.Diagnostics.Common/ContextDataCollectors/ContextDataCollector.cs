using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        readonly Collection<DataRequest> _requestedData = new Collection<DataRequest>();
        public Collection<DataRequest> RequestedData
        {
            get { return _requestedData; }
        }

        readonly Dictionary<DataRequest, DiagnosticEventProperty> _dataRequestCache
            = new Dictionary<DataRequest, DiagnosticEventProperty>();
        protected Dictionary<DataRequest, DiagnosticEventProperty> DataRequestCache
        {
            get { return _dataRequestCache; }
        }

        public abstract bool TryGetData(DataRequest dataRequest, DataCollectionContext context, out object result);

        public virtual
#if UPTO_DOTNET40
            IList<DiagnosticEventProperty>
#else
 IReadOnlyList<DiagnosticEventProperty>
#endif
 CollectData()
        {
            return CollectData(RequestedData);
        }

        public
#if UPTO_DOTNET40
            IList<DiagnosticEventProperty>
#else
 IReadOnlyList<DiagnosticEventProperty>
#endif
 CollectData(IList<DataRequest> requestedContextData)
        {
            var result = new List<DiagnosticEventProperty>(capacity: requestedContextData.Count);

            if (requestedContextData.Count == 0)
                return result;

            var cx = new DataCollectionContext();

            for (int i = 0; i < requestedContextData.Count; i++)
            {
                var item = requestedContextData[i];

                DiagnosticEventProperty property = null;

                //# try to get from cache first
                if (DataRequestCache.TryGetValue(item, out property))
                {
                    result.Add(property);
                    continue;
                }

                //# get the data
                var dataValue = (object)null;

                if (TryGetData(item, cx, out dataValue))
                {
                    property = new DiagnosticEventProperty();
                    property.UniqueName = item.Data;
                    property.Value = dataValue;
                }

                result.AddIfNotNull(property);

                // add to cache if needed
                if (item.IsCached && !DataRequestCache.ContainsKey(item))
                    DataRequestCache.Add(item, property);
            }

            return result;
        }

        public abstract IContextDataCollector Clone();
    }
}
