using SquaredInfinity.Foundation.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Diagnostics
{
    public interface IDiagnosticEventPropertyCollection :
        ICollection<IDiagnosticEventProperty>,
        IBulkUpdatesCollection<IDiagnosticEventProperty>, 
        INotifyCollectionContentChanged
    {
    }
}
