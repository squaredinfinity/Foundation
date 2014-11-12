using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation
{
    public class IndexInAnotherListComparer<TItem> : IComparer<TItem>
    {
        readonly IList<TItem> AnotherList;

        Dictionary<TItem, int> ItemToIndex_AnotherList;

        public IndexInAnotherListComparer(IList<TItem> anotherList)
        {
            this.AnotherList = anotherList;

            ProcessLists();
        }

        void ProcessLists()
        {
            //# create a mapping of each item in list to sort to index in another list

            ItemToIndex_AnotherList = new Dictionary<TItem, int>();

            for (int i = 0; i < AnotherList.Count; i++)
            {
                var item = AnotherList[i];

                if (ItemToIndex_AnotherList.ContainsKey(item))
                    continue;

                ItemToIndex_AnotherList.Add(item, i);
            }
        }

        public int Compare(TItem x, TItem y)
        {
            var x_ix = -1;
            var y_ix = -1;

            if (ItemToIndex_AnotherList.ContainsKey(x))
                x_ix = ItemToIndex_AnotherList[x];

            if (ItemToIndex_AnotherList.ContainsKey(y))
                y_ix = ItemToIndex_AnotherList[y];

            if (x_ix == -1 && y_ix == -1)
                return 0;

            if (x_ix == -1)
                return 1;

            if (y_ix == -1)
                return -1;

            return Comparer<int>.Default.Compare(x_ix, y_ix);
        }
    }
}
