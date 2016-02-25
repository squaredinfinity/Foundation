using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace SquaredInfinity.Foundation.Presentation.Controls.AdaptiveSelector
{
    public class UniformItemsSize
    {
        #region Auto Uniform Width

        public static bool GetAutoUniformWidth(ItemsControl obj)
        {
            return (bool)obj.GetValue(UseAutoUniformWidthProperty);
        }

        public static void SetAutoUniformWidth(ItemsControl obj, bool value)
        {
            obj.SetValue(UseAutoUniformWidthProperty, value);
        }

        public static readonly DependencyProperty UseAutoUniformWidthProperty =
            DependencyProperty.RegisterAttached(
            "AutoUniformWidth",
            typeof(bool),
            typeof(UniformItemsSize),
            new PropertyMetadata(false, OnAutoUniformWidthChanged));

        static void OnAutoUniformWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ic = d as ItemsControl;

            if (ic == null)
                return;

            ic.ItemContainerGenerator.ItemsChanged -= ItemContainerGenerator_ItemsChanged;
            ic.SizeChanged -= ic_SizeChanged;

            if (bool.Equals(e.NewValue, true))
            {
                ic.ItemContainerGenerator.ItemsChanged += ItemContainerGenerator_ItemsChanged;
                ic.SizeChanged += ic_SizeChanged;
            }
        }


        static void ic_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RefreshWidth(sender as ItemsControl);
        }

        static void ItemContainerGenerator_ItemsChanged(object sender, ItemsChangedEventArgs e)
        {
            RefreshWidth(sender as ItemsControl);
        }

        #endregion

        public static double GetUniformWidth(DependencyObject obj)
        {
            return (double)obj.GetValue(UniformWidthProperty);
        }

        public static void SetUniformWidth(DependencyObject obj, double value)
        {
            obj.SetValue(UniformWidthProperty, value);
        }

        public static readonly DependencyProperty UniformWidthProperty =
            DependencyProperty.RegisterAttached(
            "UniformWidth",
            typeof(double),
            typeof(UniformItemsSize),
            new PropertyMetadata(0.0));


        static void RefreshWidth(ItemsControl ic)
        {
            if (ic == null)
                return;

            var new_item_width = ic.ActualWidth / ic.Items.Count;

            SetUniformWidth(ic, new_item_width);
        }






        //static void panel_Loaded(object sender, RoutedEventArgs e)
        //{
        //    var panel = sender as Panel;

        //    // Go over the children and set margin for them:
        //    foreach (var child in panel.Children)
        //    {
        //        var fe = child as FrameworkElement;

        //        if (fe == null) continue;

        //        fe.Margin = MarginSetter.GetMargin(panel);
        //    }
        //}


    }
}
