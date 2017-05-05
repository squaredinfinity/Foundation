using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;

namespace SquaredInfinity.Extensions
{
    public static partial class TreeViewItemExtensions
    {
        public static int GetDepth(this TreeViewItem tvi)
        {
            int depth = -1;

            var item = tvi;

            do
            {
                item = GetParentTreeViewItem(item);
                depth++;
            }
            while (item != null);

            return depth;
        }

        public static TreeViewItem GetParentTreeViewItem(this TreeViewItem tvi)
        {
            var parent = VisualTreeHelper.GetParent(tvi);

            while (parent != null && !(parent is TreeViewItem) && !(parent is TreeView))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            return parent as TreeViewItem;
        }

        public static IEnumerable<TreeViewItem> GetChildTreeViewItems(this TreeViewItem tvi)
        {
            for(int i = 0; i < tvi.Items.Count; i++)
            {
                var child = tvi.ItemContainerGenerator.ContainerFromIndex(i) as TreeViewItem;

                if (child != null)
                    yield return child;
            }
        }
    }
}
