using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.Collections
{
    public class PageCollection<TDataItem> : ConcurrentDictionary<int, VirtualizedDataItemsPage<TDataItem>>
        where TDataItem : class
    { }
}
