using SquaredInfinity.Foundation.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Diagnostics
{
    public class DiagnosticEventPropertyCollection : 
        Dictionary<string, IDiagnosticEventProperty>, 
        IDiagnosticEventPropertyCollection
    {
        public DiagnosticEventPropertyCollection()
            : this(27)
        { }

        public DiagnosticEventPropertyCollection(int capacity)
            : base(capacity)
        { }

        public DiagnosticEventPropertyCollection(IEnumerable<IDiagnosticEventProperty> diagnosticEventPropertyCollection)
        {
            this.AddOrUpdateRange(diagnosticEventPropertyCollection);
        }

        public void AddOrUpdate(string propertyName, object value)
        {
            this[propertyName] = new DiagnosticEventProperty { UniqueName = propertyName, Value = value };
        }

        public void AddOrUpdate(IDiagnosticEventProperty property)
        {
            this[property.UniqueName] = property;
        }

        public void AddOrUpdateRange(IEnumerable<IDiagnosticEventProperty> properties)
        {
            foreach (var p in properties)
                this[p.UniqueName] = p;
        }

        public new IEnumerator<IDiagnosticEventProperty> GetEnumerator()
        {
            return base.Values.GetEnumerator();
        }

        public bool TryGetValue(string propertyName, out object value)
        {
            if (!this.ContainsKey(propertyName))
            {
                value = null;
                return false;
            }

            value = this[propertyName];
            return true;
        }
    }
}
