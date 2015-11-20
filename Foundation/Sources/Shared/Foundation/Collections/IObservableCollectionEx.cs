using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Collections
{
    public interface IObservableCollectionEx<TItem> : ICollectionEx<TItem>
    {
        event EventHandler<ItemAddedEventArgs<TItem>> AfterItemInserted;
        event EventHandler<ItemRemovedEventArgs<TItem>> AfterItemRemoved;
        event EventHandler<AfterItemReplacedEventArgs<TItem>> AfterItemReplaced;
    }
}
