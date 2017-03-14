using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SquaredInfinity.Presentation.Behaviors
{
    public class Click
    {
        #region CommandParameters

        public static void SetCommandParameter(UIElement element, object value)
        {
            element.SetValue(CommandParameterProperty, value);
        }

        public static object GetCommandParameter(UIElement element)
        {
            return (object)element.GetValue(CommandParameterProperty);
        }

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.RegisterAttached(
            "CommandParameter",
            typeof(object),
            typeof(Click),
            new PropertyMetadata(null));

        #endregion

        #region Command

        public static void SetCommand(UIElement element, ICommand value)
        {
            element.SetValue(CommandProperty, value);
        }

        public static ICommand GetCommand(UIElement element)
        {
            return (ICommand)element.GetValue(CommandProperty);
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached(
            "Command", 
            typeof(ICommand),
            typeof(Click), 
            new PropertyMetadata(null, OnCommandChanged));

        static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as UIElement;

            if (c == null)
                return;

            if((ICommand)e.NewValue != null)
            {
                c.PreviewMouseUp += c_PreviewMouseUp;
            }
            else
            {
                c.PreviewMouseUp -= c_PreviewMouseUp;
            }
        }

        static void c_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            var c = sender as UIElement;

            if (c == null)
                return;

            // click only captured on left button
            // todo: this may need changing depending on mouse settings (left/right handed)
            if (e.ChangedButton != MouseButton.Left || e.ButtonState != MouseButtonState.Released)
                return;

            var command = GetCommand(c);

            var parameter = GetCommandParameter(c);

            if (command != null && command.CanExecute(parameter))
            {
                command.Execute(parameter);
            }

            e.Handled = true;
        }

        #endregion
    }

    public class PreviewKeyUp
    {
        #region CommandParameters

        public static void SetCommandParameter(UIElement element, object value)
        {
            element.SetValue(CommandParameterProperty, value);
        }

        public static object GetCommandParameter(UIElement element)
        {
            return (object)element.GetValue(CommandParameterProperty);
        }

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.RegisterAttached(
            "CommandParameter",
            typeof(object),
            typeof(PreviewKeyUp),
            new PropertyMetadata(null));

        #endregion

        #region Command

        public static void SetCommand(UIElement element, ICommand value)
        {
            element.SetValue(CommandProperty, value);
        }

        public static ICommand GetCommand(UIElement element)
        {
            return (ICommand)element.GetValue(CommandProperty);
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached(
            "Command",
            typeof(ICommand),
            typeof(PreviewKeyUp),
            new PropertyMetadata(null, OnCommandChanged));

        static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as UIElement;

            if (c == null)
                return;

            if ((ICommand)e.NewValue != null)
            {
                c.PreviewKeyUp -= C_PreviewKeyUp;
                c.PreviewKeyUp += C_PreviewKeyUp;
            }
            else
            {
                c.PreviewKeyUp -= C_PreviewKeyUp;
            }
        }

        private static void C_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            var c = sender as UIElement;

            if (c == null)
                return;

            var command = GetCommand(c);
            var parameter = GetCommandParameter(c);

            var all_parameters = new Dictionary<string, object>();
            all_parameters.Add("args", e);
            all_parameters.Add("payload", parameter);

            if (command != null && command.CanExecute(all_parameters))
            {
                command.Execute(all_parameters);
            }

            e.Handled = true;
        }

        #endregion
    }

    public class KeyUp
    {
        #region CommandParameters

        public static void SetCommandParameter(UIElement element, object value)
        {
            element.SetValue(CommandParameterProperty, value);
        }

        public static object GetCommandParameter(UIElement element)
        {
            return (object)element.GetValue(CommandParameterProperty);
        }

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.RegisterAttached(
            "CommandParameter",
            typeof(object),
            typeof(KeyUp),
            new PropertyMetadata(null));

        #endregion

        #region Command

        public static void SetCommand(UIElement element, ICommand value)
        {
            element.SetValue(CommandProperty, value);
        }

        public static ICommand GetCommand(UIElement element)
        {
            return (ICommand)element.GetValue(CommandProperty);
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached(
            "Command",
            typeof(ICommand),
            typeof(KeyUp),
            new PropertyMetadata(null, OnCommandChanged));

        static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as UIElement;

            if (c == null)
                return;

            if ((ICommand)e.NewValue != null)
            {
                c.KeyUp -= _KeyUp;
                c.KeyUp += _KeyUp;
            }
            else
            {
                c.KeyUp -= _KeyUp;
            }
        }

        private static void _KeyUp(object sender, KeyEventArgs e)
        {
            var c = sender as UIElement;

            if (c == null)
                return;

            var command = GetCommand(c);
            var parameter = GetCommandParameter(c);

            var all_parameters = new Dictionary<string, object>();
            all_parameters.Add("args", e);
            all_parameters.Add("payload", parameter);

            if (command != null && command.CanExecute(all_parameters))
            {
                command.Execute(all_parameters);
            }

            e.Handled = true;
        }

        #endregion
    }







    public class PreviewKeyDown
    {
        #region CommandParameters

        public static void SetCommandParameter(UIElement element, object value)
        {
            element.SetValue(CommandParameterProperty, value);
        }

        public static object GetCommandParameter(UIElement element)
        {
            return (object)element.GetValue(CommandParameterProperty);
        }

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.RegisterAttached(
            "CommandParameter",
            typeof(object),
            typeof(PreviewKeyDown),
            new PropertyMetadata(null));

        #endregion

        #region Command

        public static void SetCommand(UIElement element, ICommand value)
        {
            element.SetValue(CommandProperty, value);
        }

        public static ICommand GetCommand(UIElement element)
        {
            return (ICommand)element.GetValue(CommandProperty);
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached(
            "Command",
            typeof(ICommand),
            typeof(PreviewKeyDown),
            new PropertyMetadata(null, OnCommandChanged));

        static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as UIElement;

            if (c == null)
                return;

            if ((ICommand)e.NewValue != null)
            {
                c.PreviewKeyDown -= C_PreviewKeyDown;
                c.PreviewKeyDown += C_PreviewKeyDown;
            }
            else
            {
                c.PreviewKeyDown -= C_PreviewKeyDown;
            }
        }

        private static void C_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var c = sender as UIElement;

            if (c == null)
                return;

            var command = GetCommand(c);
            var parameter = GetCommandParameter(c);

            var all_parameters = new Dictionary<string, object>();
            all_parameters.Add("args", e);
            all_parameters.Add("payload", parameter);

            if (command != null && command.CanExecute(all_parameters))
            {
                command.Execute(all_parameters);
            }

            e.Handled = true;
        }

        #endregion
    }

    public class KeyDown
    {
        #region CommandParameters

        public static void SetCommandParameter(UIElement element, object value)
        {
            element.SetValue(CommandParameterProperty, value);
        }

        public static object GetCommandParameter(UIElement element)
        {
            return (object)element.GetValue(CommandParameterProperty);
        }

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.RegisterAttached(
            "CommandParameter",
            typeof(object),
            typeof(KeyDown),
            new PropertyMetadata(null));

        #endregion

        #region Command

        public static void SetCommand(UIElement element, ICommand value)
        {
            element.SetValue(CommandProperty, value);
        }

        public static ICommand GetCommand(UIElement element)
        {
            return (ICommand)element.GetValue(CommandProperty);
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached(
            "Command",
            typeof(ICommand),
            typeof(KeyDown),
            new PropertyMetadata(null, OnCommandChanged));

        static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as UIElement;

            if (c == null)
                return;

            if ((ICommand)e.NewValue != null)
            {
                c.KeyDown -= _KeyDown;
                c.KeyDown += _KeyDown;
            }
            else
            {
                c.KeyDown -= _KeyDown;
            }
        }

        private static void _KeyDown(object sender, KeyEventArgs e)
        {
            var c = sender as UIElement;

            if (c == null)
                return;

            var command = GetCommand(c);
            var parameter = GetCommandParameter(c);

            var all_parameters = new Dictionary<string, object>();
            all_parameters.Add("args", e);
            all_parameters.Add("payload", parameter);

            if (command != null && command.CanExecute(all_parameters))
            {
                command.Execute(all_parameters);
            }

            e.Handled = true;
        }

        #endregion
    }
}
