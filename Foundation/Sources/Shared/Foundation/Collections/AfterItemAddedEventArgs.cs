using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Collections
{
    public class AfterItemAddedEventArgs<TItem> : EventArgs
    {
        public TItem AddedItem { get; private set; }

        public AfterItemAddedEventArgs(TItem addedItem)
        {
            this.AddedItem = addedItem;
        }
    }
}
