using SquaredInfinity.Foundation.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Diagnostics
{
    public interface IAttachedObjectCollection :
        ICollection<IAttachedObject>,
        IList<IAttachedObject>,
        IBulkUpdatesCollection<IAttachedObject>,
        INotifyCollectionContentChanged
    {

    }
}
