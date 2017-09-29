using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

namespace SquaredInfinity.Collections
{
    /// <summary>
    /// Keeps a track of all included and excluded items in the list.
    /// Allows for efficient multi-pass filtering of items without creating additional structures.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public class FilteredList<TItem> : IReadOnlyList<TItem>
    {
        readonly IReadOnlyList<TItem> Items;
        readonly BitArray Indexes;

        /// <summary>
        /// Items to be filtered.
        /// Assumes underlying items collection will never change.
        /// All items are initially excluded by default.
        /// </summary>
        /// <param name="items"></param>
        public FilteredList(IReadOnlyList<TItem> items)
        {
            Items = items;

            // all items excluded by default
            Indexes = new BitArray(Items.Count);
            Count = 0;
        }

        void _Include(int index)
        {
            if (Indexes[index])
                return;

            Indexes[index] = true;
            Count++;
        }

        void _Exclude(int index)
        {
            if (!Indexes[index])
                return;

            Indexes[index] = false;
            Count--;
        }

        /// <summary>
        /// Includes all items.
        /// </summary>
        public void IncludeAll()
        {
            Indexes.SetAll(true);
            Count = Items.Count;
        }

        /// <summary>
        /// Runs provided filter on *excluded* items.
        /// If currently excluded item passes the filter it will become inlcuded.
        /// </summary>
        /// <param name="filter"></param>
        public void Include(Func<TItem, bool> filter) => Include(filter, Items.Count);

        /// <summary>
        /// Runs provided filter on *excluded* items.
        /// If currently excluded item passes the filter it will become inlcuded.
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="maxItemsToInclude">maximum number of items to include when executing this method</param>
        public void Include(Func<TItem, bool> filter, int maxItemsToInclude)
        {
            // track how many items have been included
            var c = 0;

            for (var i = 0; i < Items.Count; i++)
            {
                if (c >= maxItemsToInclude)
                    break;

                if (Indexes[i])
                {
                    // item already included
                    continue;
                }

                if (filter(Items[i]))
                {
                    _Include(i);
                    c++;
                }
            }
        }

        /// <summary>
        /// Excludes all items.
        /// </summary>
        public void ExcludeAll()
        {
            Indexes.SetAll(false);
            Count = 0;
        }

        /// <summary>
        /// Runs provided filter on *included* items.
        /// If currently included item passes the filter it will become excluded.
        /// </summary>
        /// <param name="filter"></param>
        public void Exclude(Func<TItem, bool> filter) => Exclude(filter, Items.Count);

        /// <summary>
        /// Runs provided filter on *included* items.
        /// If currently included item passes the filter it will become excluded.
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="maxItemsToExclude">maximum number of items to exclude when executing this method</param>
        public void Exclude(Func<TItem, bool> filter, int maxItemsToExclude)
        {
            // track how many items have been excluded
            var c = 0;

            for (var i = 0; i < Items.Count; i++)
            {
                if (c >= maxItemsToExclude)
                    break;

                if (!Indexes[i])
                {
                    // item already excluded
                    continue;
                }

                if (filter(Items[i]))
                {
                    _Exclude(i);
                    c++;
                }
            }
        }

        /// <summary>
        /// Applies specified filter on all items, both those marked currently as *included* and those marked as *excluded*.
        /// </summary>
        /// <param name="filter">Items passing the filter will be included, they will be excluded otherwise</param>
        public void ResetFilter(Func<TItem, bool> filter)
        {
            for (var i = 0; i < Items.Count; i++)
            {
                if (filter(Items[i]))
                    _Include(i);
                else
                    _Exclude(i);
            }
        }

        #region IReadOnlyList

        public TItem this[int index]
        {
            get
            {
                if (index >= Count)
                    throw new IndexOutOfRangeException($"Index {index} is out of range.");

                int pos = -1;

                // find included item at specified index
                for (var i = 0; i < Items.Count; i++)
                {
                    if (Indexes[i])
                        pos++;

                    if (pos == index)
                        return Items[i];
                }

                throw new Exception($"Unable to find item with specified index {index}");
            }
        }

        public int Count { get; private set; }

        public IEnumerator<TItem> GetEnumerator() => Items.Where((item, index) => Indexes[index] == true).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Items.Where((item, index) => Indexes[index] == true).GetEnumerator();

        #endregion
    }
}
