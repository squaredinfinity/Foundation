using SquaredInfinity.Threading.Locks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Collections
{
    public interface IIndexFilter
    {
        IReadOnlyList<int> Indexes { get; }

        /// <summary>
        /// Adds specified index to the filter (item with that index will be included in results)
        /// </summary>
        /// <param name="index"></param>
        void AddIndex(int index);
        /// <summary>
        /// Removes specified index from filter (item with that index will NOT be included in results)
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        bool TryRemoveIndex(int index);

        void Reset(IReadOnlyList<int> newIndexes);
    }
}
