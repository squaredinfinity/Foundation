using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Diagnostics
{
    public interface IDiagnosticEventPropertyCollection :
        IEnumerable<IDiagnosticEventProperty>
    {
        void AddOrUpdate(string propertyName, object value);
        void AddOrUpdate(IDiagnosticEventProperty property);
        void AddOrUpdateRange(IEnumerable<IDiagnosticEventProperty> properties);

        bool TryGetValue(string propertyName, out object value);
    }
}
