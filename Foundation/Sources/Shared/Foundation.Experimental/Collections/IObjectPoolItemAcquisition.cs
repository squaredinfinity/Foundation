using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.Collections
{
    public interface IObjectPoolItemAcquisition<TItem> : IDisposable
    {
        TItem Item { get; }

        /// <summary>
        /// True if item is transient and does not need to be added back to pool upon release.
        /// This supports scenarios where there is a limited number of items in the pool and when all available items are exhausted transient items start to be created.
        /// </summary>
        bool IsItemTransient { get; }
    }
}
