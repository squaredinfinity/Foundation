using SquaredInfinity.Foundation.Diagnostics.ContextDataCollectors;
using SquaredInfinity.Foundation.Diagnostics.Filters;
using SquaredInfinity.Foundation.Diagnostics.Formatters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Diagnostics.Sinks
{
    public abstract class Sink : ISink
    {
        public string Name { get; set; }

        public bool MustWaitForWrite { get; set; }

        public IFormatter Formatter { get; set; }

        IFilter _filter;
        public IFilter Filter
        {
            get
            {
                if (_filter == null)
                    _filter = Filters.Filter.AllowAll;

                return _filter;
            }

            set
            {
                _filter = value;
            }
        }

        public abstract void Write(IDiagnosticEvent de);

        public virtual void Initialize() { }

        public abstract ISinkLocation SinkLocation { get; }

        public Sink()
        {
            MustWaitForWrite = true;
            Filter = Filters.Filter.AllowAll;
        }
        public virtual IReadOnlyList<IDataRequest> GetRequestedContextData()
        {
            if (Formatter == null)
                return new List<IDataRequest>();

            return Formatter.GetRequestedContextData();
        }


        public IList<string> Tags
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
