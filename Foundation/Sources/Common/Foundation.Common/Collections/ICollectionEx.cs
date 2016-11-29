using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Collections
{
    public interface ICollectionEx<TItem> : 
        ICollection<TItem>,
        ICollectionEx,
        IList<TI>
    {
        TItem this[int index] { get; }
        void Replace(TItem oldItem, TItem newItem);
        IReadOnlyList<TItem> GetSnapshot();
    }

    public interface ICollectionEx
    {
        void Move(int oldIndex, int newIndex);
    }
}
