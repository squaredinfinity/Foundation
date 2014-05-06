using SquaredInfinity.Foundation.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Diagnostics
{
    public class DiagnosticEventPropertyCollection : 
        CollectionEx<IDiagnosticEventProperty>, 
        IDiagnosticEventPropertyCollection
    {
        public DiagnosticEventPropertyCollection()
            : this(27)
        { }

        public DiagnosticEventPropertyCollection(int capacity)
        {
            var list = Items as List<IDiagnosticEventProperty>;
            list.Capacity = capacity;
        }
    }
}
