using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using WPFControls = System.Windows.Controls;
using SquaredInfinity.Foundation.Extensions;

namespace SquaredInfinity.Foundation.Presentation.Behaviors
{
    public partial class TreeView
    {
        #region PreventAutomaticHorizontalScrolling

        public static readonly DependencyProperty PreventAutomaticHorizontalScrollingProperty =
            DependencyProperty.RegisterAttached(
            "PreventAutomaticHorizontalScrolling",
            typeof(bool),
            typeof(TreeView),
            new PropertyMetadata(false, OnPreventAutomaticHorizontalScrollingChanged));

        public static void SetPreventAutomaticHorizontalScrolling(WPFControls.TreeView element, bool value)
        {
            element.SetValue(PreventAutomaticHorizontalScrollingProperty, value);
        }

        public static bool GetPreventAutomaticHorizontalScrolling(WPFControls.TreeView element)
        {
            return (bool)element.GetValue(PreventAutomaticHorizontalScrollingProperty);
        }

        #endregion

        #region PreventAutomaticVerticalScrolling

        public static readonly DependencyProperty PreventAutomaticVerticalScrollingProperty =
            DependencyProperty.RegisterAttached(
            "PreventAutomaticVerticalScrolling",
            typeof(bool),
            typeof(TreeView),
            new PropertyMetadata(false, OnPreventAutomaticVerticalScrollingChanged));

        public static void SetPreventAutomaticVerticalScrolling(WPFControls.TreeView element, bool value)
        {
            element.SetValue(PreventAutomaticVerticalScrollingProperty, value);
        }

        public static bool GetPreventAutomaticVerticalScrolling(WPFControls.TreeView element)
        {
            return (bool)element.GetValue(PreventAutomaticVerticalScrollingProperty);
        }

        #endregion

        #region CancelNextHorizontalScrollEvent

        public static readonly DependencyProperty CancelNextHorizontalScrollEventProperty =
        DependencyProperty.RegisterAttached(
        "CancelNextHorizontalScrollEvent",
        typeof(bool),
        typeof(TreeView),
        new PropertyMetadata(false));

        public static void SetCancelNextHorizontalScrollEvent(WPFControls.TreeView element, bool value)
        {
            element.SetValue(CancelNextHorizontalScrollEventProperty, value);
        }

        public static bool GetCancelNextHorizontalScrollEvent(WPFControls.TreeView element)
        {
            return (bool)element.GetValue(CancelNextHorizontalScrollEventProperty);
        }

        #endregion

        #region CancelNextVerticalScrollEvent

        public static readonly DependencyProperty CancelNextVerticalScrollEventProperty =
        DependencyProperty.RegisterAttached(
        "CancelNextVerticalScrollEvent",
        typeof(bool),
        typeof(TreeView),
        new PropertyMetadata(false));

        public static void SetCancelNextVerticalScrollEvent(WPFControls.TreeView element, bool value)
        {
            element.SetValue(CancelNextVerticalScrollEventProperty, value);
        }

        public static bool GetCancelNextVerticalScrollEvent(WPFControls.TreeView element)
        {
            return (bool)element.GetValue(CancelNextVerticalScrollEventProperty);
        }

        #endregion

        #region OriginalHorisontalOffset

        public static readonly DependencyProperty OriginalHorizontalOffsetProperty =
        DependencyProperty.RegisterAttached(
        "OriginalHorizontalOffset",
        typeof(double),
        typeof(TreeView),
        new PropertyMetadata(0.0));

        public static void SetOriginalHorizontalOffset(WPFControls.TreeView element, double value)
        {
            element.SetValue(OriginalHorizontalOffsetProperty, value);
        }

        public static double GetOriginalHorizontalOffset(WPFControls.TreeView element)
        {
            return (double)element.GetValue(OriginalHorizontalOffsetProperty);
        }

        #endregion

        #region OriginalVerticalOffset

        public static readonly DependencyProperty OriginalVerticalOffsetProperty =
        DependencyProperty.RegisterAttached(
        "OriginalVerticalOffset",
        typeof(double),
        typeof(TreeView),
        new PropertyMetadata(0.0));

        public static void SetOriginalVerticalOffset(WPFControls.TreeView element, double value)
        {
            element.SetValue(OriginalVerticalOffsetProperty, value);
        }

        public static double GetOriginalVerticalOffset(WPFControls.TreeView element)
        {
            return (double)element.GetValue(OriginalVerticalOffsetProperty);
        }

        #endregion

        static void OnPreventAutomaticHorizontalScrollingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tv = d as WPFControls.TreeView;

            if (tv.IsLoaded)
            {
                UpdatePreventAutomaticHorizontalScrollingSate(tv, (bool)e.NewValue);
            }
            else
            {
                tv.Loaded -= PreventAutomaticScrolling_TreeViewLoaded;
                tv.Loaded += PreventAutomaticScrolling_TreeViewLoaded;
            }
        }

        static void OnPreventAutomaticVerticalScrollingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tv = d as WPFControls.TreeView;

            if (tv.IsLoaded)
            {
                UpdatePreventAutomaticVerticalScrollingSate(tv, (bool)e.NewValue);
            }
            else
            {
                tv.Loaded -= PreventAutomaticScrolling_TreeViewLoaded;
                tv.Loaded += PreventAutomaticScrolling_TreeViewLoaded;
            }
        }

        static void UpdatePreventAutomaticHorizontalScrollingSate(WPFControls.TreeView tv, bool isEnabled)
        {
            if (isEnabled)
            {
                var scrollViewer = tv.FindVisualDescendant<WPFControls.ScrollViewer>();

                if (scrollViewer == null)
                    return;

                scrollViewer.ScrollChanged += scrollViewer_HorizontalScrollChanged;
                tv.RequestBringIntoView += tv_HorizontalRequestBringIntoView;
            }
            else
            {
                var scrollViewer = tv.FindVisualDescendant<WPFControls.ScrollViewer>();

                if (scrollViewer == null)
                    return;

                scrollViewer.ScrollChanged -= scrollViewer_HorizontalScrollChanged;
                tv.RequestBringIntoView -= tv_HorizontalRequestBringIntoView;
            }
        }

        static void UpdatePreventAutomaticVerticalScrollingSate(WPFControls.TreeView tv, bool isEnabled)
        {
            if (isEnabled)
            {
                var scrollViewer = tv.FindVisualDescendant<WPFControls.ScrollViewer>();

                if (scrollViewer == null)
                    return;

                scrollViewer.ScrollChanged += scrollViewer_VerticalScrollChanged;
                tv.RequestBringIntoView += tv_VerticalRequestBringIntoView;
            }
            else
            {
                var scrollViewer = tv.FindVisualDescendant<WPFControls.ScrollViewer>();

                if (scrollViewer == null)
                    return;

                scrollViewer.ScrollChanged -= scrollViewer_VerticalScrollChanged;
                tv.RequestBringIntoView -= tv_VerticalRequestBringIntoView;
            }
        }

        static void PreventAutomaticScrolling_TreeViewLoaded(object sender, EventArgs e)
        {
            var tv = sender as WPFControls.TreeView;

            tv.Loaded -= PreventAutomaticScrolling_TreeViewLoaded;
            UpdatePreventAutomaticHorizontalScrollingSate(tv, GetPreventAutomaticHorizontalScrolling(tv));
            UpdatePreventAutomaticVerticalScrollingSate(tv, GetPreventAutomaticVerticalScrolling(tv));
        }

        static void tv_HorizontalRequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            var tv = sender as WPFControls.TreeView;
            if(tv == null)
                return;

            var scrollViewer = tv.FindVisualDescendant<WPFControls.ScrollViewer>();
            if (scrollViewer == null)
                return;

            SetCancelNextHorizontalScrollEvent(tv, true);
            SetOriginalHorizontalOffset(tv, scrollViewer.HorizontalOffset);
        }

        static void tv_VerticalRequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            var tv = sender as WPFControls.TreeView;
            if (tv == null)
                return;

            var scrollViewer = tv.FindVisualDescendant<WPFControls.ScrollViewer>();
            if (scrollViewer == null)
                return;

            SetCancelNextVerticalScrollEvent(tv, true);
            SetOriginalVerticalOffset(tv, scrollViewer.VerticalOffset);
        }

        static void scrollViewer_HorizontalScrollChanged(object sender, WPFControls.ScrollChangedEventArgs e)
        {
            var sv = sender as WPFControls.ScrollViewer;
            var tv = sv.FindVisualParent<WPFControls.TreeView>();
            if (tv == null)
                return;

            if (!GetCancelNextHorizontalScrollEvent(tv))
            {
                SetCancelNextHorizontalScrollEvent(tv, false);
                return;
            }

            var originalHorizontalOffset = GetOriginalHorizontalOffset(tv);
            if (originalHorizontalOffset != sv.HorizontalOffset)
            {
                sv.ScrollToHorizontalOffset(GetOriginalHorizontalOffset(tv));
            }

            SetCancelNextHorizontalScrollEvent(tv, false);
        }

        static void scrollViewer_VerticalScrollChanged(object sender, WPFControls.ScrollChangedEventArgs e)
        {
            var sv = sender as WPFControls.ScrollViewer;
            var tv = sv.FindVisualParent<WPFControls.TreeView>();
            if (tv == null)
                return;

            if (!GetCancelNextVerticalScrollEvent(tv))
            {
                SetCancelNextVerticalScrollEvent(tv, false);
                return;
            }

            var originalVerticalOffset = GetOriginalVerticalOffset(tv);
            if (originalVerticalOffset != sv.VerticalOffset)
            {
                sv.ScrollToVerticalOffset(GetOriginalVerticalOffset(tv));
            }            

            SetCancelNextVerticalScrollEvent(tv, false);
        }
    }
}
