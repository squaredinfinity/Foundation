using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Collections
{
    public class AfterItemReplacedEventArgs<TItem> : EventArgs
    {
        public int Index { get; private set; }
        public TItem OldItem { get; private set; }
        public TItem NewItem { get; private set; }

        public AfterItemReplacedEventArgs(int index, TItem oldItem, TItem newItem)
        {
            this.Index = index;
            this.OldItem = oldItem;
            this.NewItem = newItem;
        }
    }
}
