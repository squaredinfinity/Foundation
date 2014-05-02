using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Collections
{
    public interface INotifyCollectionContentChanged
    {
        /// <summary>
        /// Occurs when content of any item in the collection has changed (but no items have been added / removed / moved).
        /// </summary>
        event Action<INotifyCollectionContentChanged> VersionChanged;

        int Version { get; }
    }
}
