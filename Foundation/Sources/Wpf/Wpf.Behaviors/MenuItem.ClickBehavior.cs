using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace  xxx.Foundation.Presentation.Behaviors
{
    public static partial class MenuItemBehaviors
    {
        public class MenuItemClickCommandBehavior : CommandBehaviorBase<MenuItem>
        {
            public MenuItemClickCommandBehavior(MenuItem mi)
                : base(mi)
            {
                mi.Click += mi_Click;
            }

            void mi_Click(object sender, RoutedEventArgs e)
            {
                ExecuteCommand();
            }
        }
    }
}
