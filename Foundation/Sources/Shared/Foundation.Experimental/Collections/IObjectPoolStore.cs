using System;
using System.Collections.Generic;
using System.Text;

namespace Experimental.Collections
{
    public interface IObjectPoolStore<TItem>
    {
        IObjectPoolItemAcquisition<TItem> Acquire();
        void Release(IObjectPoolItemAcquisition<TItem> item);
    }
}
