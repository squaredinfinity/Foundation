using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using WPFControls = System.Windows.Controls;

namespace SquaredInfinity.Presentation.Behaviors
{
    public partial class TreeView
    {
        static readonly Type TreeViewType;
        static readonly PropertyInfo SelectedContainerProperty;
        static readonly PropertyInfo IsSelectionChangeActiveProperty;

        static TreeView()
        {
            TreeViewType = typeof(WPFControls.TreeView);
            IsSelectionChangeActiveProperty = TreeViewType.GetProperty("IsSelectionChangeActive", BindingFlags.NonPublic | BindingFlags.Instance);
            SelectedContainerProperty = TreeViewType.GetProperty("SelectedContainer", BindingFlags.Instance | BindingFlags.NonPublic);
        }
    }
}
