using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Collections
{
    public class ItemAddedEventArgs<TItem> : EventArgs
    {
        public TItem AddedItem { get; private set; }

        public ItemAddedEventArgs(TItem addedItem)
        {
            this.AddedItem = addedItem;
        }
    }
}
