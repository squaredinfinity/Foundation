using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Diagnostics
{
    public interface IAttachedObjectCollection :
        ICollection<IAttachedObject>,
        IList<IAttachedObject>//,
        //IBulkUpdatesCollection<IAttachedObject>,
        //INotifyCollectionContentChanged
    {

    }
}
