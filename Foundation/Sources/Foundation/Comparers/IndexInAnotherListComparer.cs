using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Comparers
{
    public class IndexInAnotherListComparer<TItem> : CompositeComparer<TItem>
    {
        public IndexInAnotherListComparer(IReadOnlyList<TItem> listToSort, IReadOnlyList<TItem> anotherList)
        {
            var toSortComparer = new IndexInListComparer<TItem>(listToSort);
            var anotherComparer = new IndexInListComparer<TItem>(anotherList);

            Comparisons.Add((x, y) => anotherComparer.Compare(x, y));
            Comparisons.Add((x, y) => toSortComparer.Compare(x, y));
        }
    }
}
