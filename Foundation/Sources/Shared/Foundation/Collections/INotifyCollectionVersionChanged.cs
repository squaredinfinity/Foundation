using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Collections
{
    public interface INotifyCollectionVersionChanged
    {
        /// <summary>
        /// Occurs when content of any item in the collection has changed (but no items have been added / removed / moved).
        /// </summary>
        event EventHandler<VersionChangedEventArgs> VersionChanged;

        long Version { get; }
    }
}
