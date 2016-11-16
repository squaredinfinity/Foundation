using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace  xxx.Foundation.Presentation.Behaviors
{
    public static partial class ControlBehaviors
    {
        public class DoubleClickBehavior : CommandBehaviorBase<Control>
        {
            public DoubleClickBehavior(Control element)
                : base(element)
            {
                element.MouseDoubleClick += OnMouseDoubleClick;
            }

            void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
            {
                if (Command != null && Command.CanExecute(CommandParameter))
                {
                    ExecuteCommand();
                    e.Handled = true;
                }
            }
        }
    }
}
