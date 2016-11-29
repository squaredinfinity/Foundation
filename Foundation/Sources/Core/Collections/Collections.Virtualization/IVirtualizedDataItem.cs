using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.Foundation.Collections
{
    public interface IVirtualizedDataItem<TDataItem>
       where TDataItem : class
    {
        int Index { get; set; }
        bool IsLoading { get; set; }
        TDataItem DataItem { get; set; }
    }
}
