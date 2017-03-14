using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.Collections
{
    public class VirtualizedDataItemsPage<TDataItem>
            where TDataItem : class
    {
        IList<IVirtualizedDataItem<TDataItem>> _items;
        public IList<IVirtualizedDataItem<TDataItem>> Items
        {
            get { return _items; }
            private set { _items = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startIndex">start index of first item on this page in relation to a whole underlying collection being virtualized</param>
        /// <param name="pageSize">number of items in this page</param>
        public VirtualizedDataItemsPage(int startIndex, int pageSize)
        {
            Items = new List<IVirtualizedDataItem<TDataItem>>(capacity: pageSize);

            for (int i = 0; i < pageSize; i++)
            {
                Items.Add(new VirtualizedDataItem<TDataItem>(index: i));
            }
        }

        /// <summary>
        /// Updates this page with new items.
        /// </summary>
        /// <param name="newItems"></param>
        public void ReloadItems(IReadOnlyList<TDataItem> newItems)
        {
            //# Replace any existing items

            var to_count = Math.Min(newItems.Count, Items.Count);

            for (int i = 0; i < to_count; i++)
            {
                if (!object.Equals(Items[i], newItems[i]))
                    Items[i].DataItem = newItems[i];
            }

            //# Add new items

            var dataItem_index = Items[to_count - 1].Index;

            for (int i = to_count; i < newItems.Count; i++)
            {
                var di = new VirtualizedDataItem<TDataItem>(dataItem_index + i);
                di.DataItem = newItems[i];

                Items.Add(di);
            }

            //# Remove remaining old items
            for (int i = Items.Count - 1; i >= newItems.Count; i--)
            {
                Items.RemoveAt(i);
            }
        }
    }
}
