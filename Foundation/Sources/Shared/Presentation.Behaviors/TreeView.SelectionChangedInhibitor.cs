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
        class SelectionChangedInhibitor : IDisposable
        {
            WPFControls.TreeView Target;
            bool OriginalIsSelectionChangeActiveValue;

            public SelectionChangedInhibitor(WPFControls.TreeView treeView)
            {
                this.Target = treeView;

                OriginalIsSelectionChangeActiveValue = (bool)IsSelectionChangeActiveProperty.GetValue(Target, null);

                //# suppress selection change notification
                IsSelectionChangeActiveProperty.SetValue(Target, true, null);
            }

            public void Dispose()
            {
                IsSelectionChangeActiveProperty.SetValue(Target, OriginalIsSelectionChangeActiveValue, null);
            }
        }
    }
}
