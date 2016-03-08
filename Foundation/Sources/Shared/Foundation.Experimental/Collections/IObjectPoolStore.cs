using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.Foundation.Collections
{
    public interface IObjectPoolStore<TItem>
    {
        IObjectPoolItemAcquisition<TItem> Acquire();
        bool TryAcquireItem(TItem item, out IObjectPoolItemAcquisition<TItem> acquisition);

        void Release(IObjectPoolItemAcquisition<TItem> item);
    }
}
