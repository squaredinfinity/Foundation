using SquaredInfinity.Foundation.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Diagnostics.Sinks
{
    public interface ISinkCollection : ICollection<ISink>, IBulkUpdatesCollection<ISink>, INotifyCollectionContentChanged
    {
    }
}
