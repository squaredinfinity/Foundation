using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.Collections
{
    public interface IVirtualizedDataItemsProvider<TDataItem>
    {
        event EventHandler AfterDataChanged;

        /// <summary>
        /// Returns a total number of all items in an underlying collection being virtualized
        /// </summary>
        /// <returns></returns>
        int GetTotalItemsCount();

        IReadOnlyList<TDataItem> GetItems(int startIndex, int numberOfItems);
    }
}
