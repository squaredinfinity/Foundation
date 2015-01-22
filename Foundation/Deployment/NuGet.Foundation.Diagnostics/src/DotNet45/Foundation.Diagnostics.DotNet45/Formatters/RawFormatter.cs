using SquaredInfinity.Foundation.Diagnostics.ContextDataCollectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Diagnostics.Formatters
{
    public class RawFormatter : IFormatter
    {
        public string Name { get; set; }

        public string Format(IDiagnosticEvent de)
        {
            return de.Message;
        }

        public IReadOnlyList<IDataRequest> GetRequestedContextData()
        {
            return new List<IDataRequest>();
        }

        public void Initialize()
        {
        }
    }
}
