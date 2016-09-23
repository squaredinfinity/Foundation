using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation
{
    /// <summary>
    /// Compares indexes of items in the list.
    /// Any subsequent changes to the list will not be reflected (i.e. indexes of items are cached internally when instance of this class is created)
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public class IndexInListComparer<TItem> : IComparer<TItem>
    {
        readonly IReadOnlyList<TItem> TheList;
        readonly Dictionary<TItem, int> ItemToIndex;

        public IndexInListComparer(IReadOnlyList<TItem> list)
        {
            this.TheList = list;

            ItemToIndex = ProcessList(list);
        }

        Dictionary<TItem, int> ProcessList(IReadOnlyList<TItem> list)
        {
            //# create a mapping of each item in list to sort to index in another list

            var dict = new Dictionary<TItem, int>();

            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];

                if (dict.ContainsKey(item))
                {
                    throw new InvalidOperationException("IndexInListComparer only supports list with distinct items.");
                }

                dict.Add(item, i);
            }

            return dict;
        }

        public int Compare(TItem x, TItem y)
        {
            var x_ix = -1;
            var y_ix = -1;

            if (ItemToIndex.ContainsKey(x))
                x_ix = ItemToIndex[x];

            if (ItemToIndex.ContainsKey(y))
                y_ix = ItemToIndex[y];

            if (x_ix == -1 && y_ix == -1)
            {
                return 0;
            }

            if (x_ix == -1)
            {
                return 1;
            }

            if (y_ix == -1)
                return -1;

            return Comparer<int>.Default.Compare(x_ix, y_ix);
        }
    }
}
