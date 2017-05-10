using SquaredInfinity.Threading;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SquaredInfinity.Windows.Behaviors
{
    public class SingleOrDoubleClick
    {
        #region Invocation Throttle

        static void SetInvocationThrottle(UIElement element, InvocationThrottle value)
        {
            element.SetValue(InvocationThrottleProperty, value);
        }

        static InvocationThrottle GetInvocationThrottle(UIElement element)
        {
            return (InvocationThrottle)element.GetValue(InvocationThrottleProperty);
        }

        static readonly DependencyProperty InvocationThrottleProperty =
            DependencyProperty.RegisterAttached(
            "InvocationThrottle",
            typeof(InvocationThrottle),
            typeof(SingleOrDoubleClick),
            new PropertyMetadata(new InvocationThrottle(250, 500)));

        #endregion

        #region Click Count

        static void SetClickCount(UIElement element, ClickCount value)
        {
            element.SetValue(ClickCountProperty, value);
        }

        static ClickCount GetClickCount(UIElement element)
        {
            return (ClickCount)element.GetValue(ClickCountProperty);
        }

        static readonly DependencyProperty ClickCountProperty =
            DependencyProperty.RegisterAttached(
            "ClickCount",
            typeof(ClickCount),
            typeof(SingleOrDoubleClick),
            new PropertyMetadata(new ClickCount()));

        #endregion

        #region Set Event Handled

        public static void SetSetEventHandled(UIElement element, bool value)
        {
            element.SetValue(SetEventHandledProperty, value);
        }

        public static bool GetSetEventHandled(UIElement element)
        {
            return (bool)element.GetValue(SetEventHandledProperty);
        }

        public static readonly DependencyProperty SetEventHandledProperty =
            DependencyProperty.RegisterAttached(
            "SetEventHandled",
            typeof(bool),
            typeof(SingleOrDoubleClick),
            new PropertyMetadata(true));

        #endregion

        #region Single Click Command Parameter

        public static void SetSingleClickCommandParameter(UIElement element, object value)
        {
            element.SetValue(SingleClickCommandParameterProperty, value);
        }

        public static object GetSingleClickCommandParameter(UIElement element)
        {
            return (object)element.GetValue(SingleClickCommandParameterProperty);
        }

        public static readonly DependencyProperty SingleClickCommandParameterProperty =
            DependencyProperty.RegisterAttached(
            "SingleClickCommandParameter",
            typeof(object),
            typeof(SingleOrDoubleClick),
            new PropertyMetadata(null));

        #endregion

        #region Double Click Command Parameter

        public static void SetDoubleClickCommandParameter(UIElement element, object value)
        {
            element.SetValue(DoubleClickCommandParameterProperty, value);
        }

        public static object GetDoubleClickCommandParameter(UIElement element)
        {
            return (object)element.GetValue(DoubleClickCommandParameterProperty);
        }

        public static readonly DependencyProperty DoubleClickCommandParameterProperty =
            DependencyProperty.RegisterAttached(
            "DoubleClickCommandParameter",
            typeof(object),
            typeof(SingleOrDoubleClick),
            new PropertyMetadata(null));

        #endregion

        #region Single Click Command

        public static void SetSingleClickCommand(UIElement element, ICommand value)
        {
            element.SetValue(SingleClickCommandProperty, value);
        }

        public static ICommand GetSingleClickCommand(UIElement element)
        {
            return (ICommand)element.GetValue(SingleClickCommandProperty);
        }

        public static readonly DependencyProperty SingleClickCommandProperty =
            DependencyProperty.RegisterAttached(
            "SingleClickCommand", 
            typeof(ICommand),
            typeof(SingleOrDoubleClick), 
            new PropertyMetadata(null, OnCommandChanged));

        #endregion

        #region Double Click Command

        public static void SetDoubleClickCommand(UIElement element, ICommand value)
        {
            element.SetValue(DoubleClickCommandProperty, value);
        }

        public static ICommand GetDoubleClickCommand(UIElement element)
        {
            return (ICommand)element.GetValue(DoubleClickCommandProperty);
        }

        public static readonly DependencyProperty DoubleClickCommandProperty =
            DependencyProperty.RegisterAttached(
            "DoubleClickCommand",
            typeof(ICommand),
            typeof(SingleOrDoubleClick),
            new PropertyMetadata(null, OnCommandChanged));
        
        #endregion

        
        static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as UIElement;

            if (c == null)
                return;

            if((ICommand)e.NewValue != null)
            {
                c.MouseUp -= C_MouseUp;
                c.MouseUp += C_MouseUp;
            }
            else
            {
                c.MouseUp -= C_MouseUp;
            }
        }

        private static void C_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var c = sender as UIElement;

            if (c == null)
                return;

            // click only captured on left button
            // todo: this may need changing depending on mouse settings (left/right handed)
            if (e.ChangedButton != MouseButton.Left || e.ButtonState != MouseButtonState.Released)
                return;

            var set_event_handled = GetSetEventHandled(c);
            var throttle = GetInvocationThrottle(c);
            var click_count = GetClickCount(c);
            click_count.Increment();


            throttle.InvokeAsync(
                DoClick,
                new State
                {
                    ClickCount = click_count,
                    SingleClickCommand = GetSingleClickCommand(c),
                    DoubleClickCommand = GetDoubleClickCommand(c),
                    SingleClickParameter = GetSingleClickCommandParameter(c),
                    DoubleClickParameter = GetDoubleClickCommandParameter(c)
                });

            if(set_event_handled)
                e.Handled = true;
        }

        static void DoClick(object state, InvocationThrottleWorkItem wi, CancellationToken ct)
        {
            var s = state as State;

            var command = (ICommand)null;
            var parameter = (object)null;

            if (s.ClickCount.Value < 2)
            {
                command = s.SingleClickCommand;
                parameter = s.SingleClickParameter;
            }
            else
            {
                command = s.DoubleClickCommand;
                parameter = s.DoubleClickParameter;
            }

            if (command != null && command.CanExecute(parameter))
            {
                command.Execute(parameter);
            }

            s.ClickCount.Reset();
        }

        class State
        {
            public ClickCount ClickCount;

            public ICommand SingleClickCommand;
            public ICommand DoubleClickCommand;
            public object SingleClickParameter;
            public object DoubleClickParameter;
        }
    }
}
