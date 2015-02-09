using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Collections
{
    public class AfterItemRemovedEventArgs<TItem> : EventArgs
    {
        public TItem RemovedItem { get; private set; }

        public AfterItemRemovedEventArgs(TItem removedItem)
        {
            this.RemovedItem = removedItem;
        }
    }
}
