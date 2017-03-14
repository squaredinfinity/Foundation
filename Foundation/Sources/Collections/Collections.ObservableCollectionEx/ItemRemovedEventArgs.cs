using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Collections
{
    public class ItemRemovedEventArgs<TItem> : EventArgs
    {
        public TItem RemovedItem { get; private set; }

        public ItemRemovedEventArgs(TItem removedItem)
        {
            this.RemovedItem = removedItem;
        }
    }
}
