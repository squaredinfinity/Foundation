using SquaredInfinity.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows;

namespace  xxx.Foundation.Presentation.Behaviors
{
    public static partial class UIElementBehaviors
    {
        #region MouseLeftButtonUp Command

        public static ICommand GetMouseLeftButtonUpCommand(System.Windows.UIElement uie)
        {
            return (ICommand)uie.GetValue(MouseLeftButtonUpCommandProperty);
        }

        public static void SetMouseLeftButtonUpCommand(System.Windows.UIElement uie, ICommand value)
        {
            uie.SetValue(MouseLeftButtonUpCommandProperty, value);
        }

        public static readonly DependencyProperty MouseLeftButtonUpCommandProperty =
            DependencyProperty.RegisterAttached(
            "MouseLeftButtonUpCommand",
            typeof(ICommand),
            typeof(UIElementBehaviors),
            new PropertyMetadata(null, OnMouseLeftButtonUpCommandChanged));

        static void OnMouseLeftButtonUpCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uie = d as System.Windows.UIElement;

            var behavior = GetOrCreateUIElementMouseLeftButtonUpCommandBehavior(uie);

            behavior.Command = e.NewValue as ICommand;
        }

        #endregion

        #region MouseLeftButtonUp Command Parameter

        public static object GetMouseLeftButtonUpCommandParameter(System.Windows.UIElement mi)
        {
            return (object)mi.GetValue(MouseLeftButtonUpCommandParameterProperty);
        }

        public static void SetMouseLeftButtonUpCommandParameter(System.Windows.UIElement mi, object value)
        {
            mi.SetValue(MouseLeftButtonUpCommandParameterProperty, value);
        }

        public static readonly DependencyProperty MouseLeftButtonUpCommandParameterProperty =
            DependencyProperty.RegisterAttached(
            "MouseLeftButtonUpCommandParameter",
            typeof(object),
            typeof(UIElementBehaviors),
            new PropertyMetadata(null, OnMouseLeftButtonUpCommandParameterChanged));

        static void OnMouseLeftButtonUpCommandParameterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uie = d as System.Windows.UIElement;

            var behavior = GetOrCreateUIElementMouseLeftButtonUpCommandBehavior(uie);

            behavior.CommandParameter = e.NewValue;
        }

        #endregion

        #region CommandStateChangeTrigger

        public static object GetCommandStateChangeTrigger(System.Windows.UIElement uie)
        {
            return (object)uie.GetValue(CommandStateChangeTriggerProperty);
        }

        public static void SetCommandStateChangeTrigger(System.Windows.UIElement uie, object value)
        {
            uie.SetValue(CommandStateChangeTriggerProperty, value);
        }

        public static readonly DependencyProperty CommandStateChangeTriggerProperty =
            DependencyProperty.RegisterAttached(
            "CommandStateChangeTrigger",
            typeof(object),
            typeof(UIElementBehaviors),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnCommandStateChangeTriggerChanged));

        static void OnCommandStateChangeTriggerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var mi = d as System.Windows.UIElement;

            var behavior = GetOrCreateUIElementMouseLeftButtonUpCommandBehavior(mi);

            behavior.CommandStateChangeTrigger = e.NewValue;
        }

        #endregion

        #region MenuItemClickCommandBehavior

        static readonly DependencyProperty MenuItemClickCommandBehaviorProperty =
            DependencyProperty.RegisterAttached(
            "UIElementMouseLeftButtonUpCommandBehavior",
            typeof(UIElementMouseLeftButtonUpCommandBehavior),
            typeof(UIElementBehaviors),
            new PropertyMetadata());

        static UIElementMouseLeftButtonUpCommandBehavior GetMenuItemClickCommandBehavior(System.Windows.UIElement mi)
        {
            return mi.GetValue(MenuItemClickCommandBehaviorProperty) as UIElementMouseLeftButtonUpCommandBehavior;
        }

        static void SetMenuItemClickCommandBehavior(System.Windows.UIElement mi, UIElementMouseLeftButtonUpCommandBehavior value)
        {
            mi.SetValue(MenuItemClickCommandBehaviorProperty, value);
        }

        static UIElementMouseLeftButtonUpCommandBehavior GetOrCreateUIElementMouseLeftButtonUpCommandBehavior(System.Windows.UIElement uie)
        {
            var behavior = GetMenuItemClickCommandBehavior(uie);

            if (behavior == null)
            {
                behavior = new UIElementMouseLeftButtonUpCommandBehavior(uie);
                SetMenuItemClickCommandBehavior(uie, behavior);
            }

            return behavior;
        }


        #endregion

        static void ExecuteClickCommand(object sender, RoutedEventArgs e)
        {
            var uie = sender as System.Windows.UIElement;

            var command = GetMouseLeftButtonUpCommand(uie);
            var parameter = GetMouseLeftButtonUpCommandParameter(uie);

            command.Execute(parameter);
        }
    }
}
    