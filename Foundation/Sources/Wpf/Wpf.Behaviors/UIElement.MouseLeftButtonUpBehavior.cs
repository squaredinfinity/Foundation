using SquaredInfinity.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace  xxx.Foundation.Presentation.Behaviors
{
    public static partial class UIElementBehaviors
    {
        public class UIElementMouseLeftButtonUpCommandBehavior : CommandBehaviorBase<System.Windows.UIElement>
        {
            public UIElementMouseLeftButtonUpCommandBehavior(System.Windows.UIElement mi)
                : base(mi)
            {
                mi.MouseLeftButtonUp += mi_MouseLeftButtonUp;
            }

            void mi_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
            {
                ExecuteCommand();
            }
        }
    }
}
    