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
        #region Click Command

        public static ICommand GetClickCommand(MenuItem mi)
        {
            return (ICommand)mi.GetValue(ClickCommandProperty);
        }

        public static void SetClickCommand(MenuItem mi, ICommand value)
        {
            mi.SetValue(ClickCommandProperty, value);
        }

        public static readonly DependencyProperty ClickCommandProperty =
            DependencyProperty.RegisterAttached(
            "ClickCommand",
            typeof(ICommand),
            typeof(MenuItemBehaviors),
            new PropertyMetadata(null, OnClickCommandChanged));

        static void OnClickCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var mi = d as MenuItem;

            var behavior = GetOrCreateMenuItemClickCommandBehavior(mi);

            behavior.Command = e.NewValue as ICommand;
        }

        #endregion

        #region Click Command Parameter

        public static object GetClickCommandParameter(MenuItem mi)
        {
            return (object)mi.GetValue(ClickCommandParameterProperty);
        }

        public static void SetClickCommandParameter(MenuItem mi, object value)
        {
            mi.SetValue(ClickCommandParameterProperty, value);
        }

        public static readonly DependencyProperty ClickCommandParameterProperty =
            DependencyProperty.RegisterAttached(
            "ClickCommandParameter",
            typeof(object),
            typeof(MenuItemBehaviors),
            new PropertyMetadata(null, OnClickCommandParameterChanged));

        static void OnClickCommandParameterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var mi = d as MenuItem;

            var behavior = GetOrCreateMenuItemClickCommandBehavior(mi);

            behavior.CommandParameter = e.NewValue;
        }

        #endregion

        #region CommandStateChangeTrigger

        public static object GetCommandStateChangeTrigger(MenuItem mi)
        {
            return (object)mi.GetValue(CommandStateChangeTriggerProperty);
        }

        public static void SetCommandStateChangeTrigger(MenuItem mi, object value)
        {
            mi.SetValue(CommandStateChangeTriggerProperty, value);
        }

        public static readonly DependencyProperty CommandStateChangeTriggerProperty =
            DependencyProperty.RegisterAttached(
            "CommandStateChangeTrigger",
            typeof(object),
            typeof(MenuItemBehaviors),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnCommandStateChangeTriggerChanged));

        static void OnCommandStateChangeTriggerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var mi = d as MenuItem; 

            var behavior = GetOrCreateMenuItemClickCommandBehavior(mi);

            behavior.CommandStateChangeTrigger = e.NewValue;
        }

        #endregion

        #region MenuItemClickCommandBehavior

        static readonly DependencyProperty MenuItemClickCommandBehaviorProperty =
            DependencyProperty.RegisterAttached(
            "MenuItemClickCommandBehavior",
            typeof(MenuItemClickCommandBehavior),
            typeof(MenuItemBehaviors),
            new PropertyMetadata());

        static MenuItemClickCommandBehavior GetMenuItemClickCommandBehavior(MenuItem mi)
        {
            return mi.GetValue(MenuItemClickCommandBehaviorProperty) as MenuItemClickCommandBehavior;
        }

        static void SetMenuItemClickCommandBehavior(MenuItem mi, MenuItemClickCommandBehavior value)
        {
            mi.SetValue(MenuItemClickCommandBehaviorProperty, value);
        }

        static MenuItemClickCommandBehavior GetOrCreateMenuItemClickCommandBehavior(MenuItem mi)
        {
            var behavior = GetMenuItemClickCommandBehavior(mi);

            if (behavior == null)
            {
                behavior = new MenuItemClickCommandBehavior(mi);
                SetMenuItemClickCommandBehavior(mi, behavior);
            }

            return behavior;
        }


        #endregion

        static void ExecuteClickCommand(object sender, RoutedEventArgs e)
        {
            var mi = sender as MenuItem;

            var command = GetClickCommand(mi);
            var parameter = GetClickCommandParameter(mi);

            command.Execute(parameter);
        }
    }
}
