using SquaredInfinity.Foundation.Graphics.ColorSpaces;
using SquaredInfinity.Foundation.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace SquaredInfinity.Foundation.Presentation.Behaviors
{
    public static class Highlight
    {

        #region Selection Highlight Brush

        public static Brush GetSelectionHighlightBrush(DependencyObject obj)
        {
            return (Brush)obj.GetValue(SelectionHighlightBrushProperty);
        }

        public static void SetSelectionHighlightBrush(DependencyObject obj, Brush value)
        {
            obj.SetValue(SelectionHighlightBrushProperty, value);
        }

        /// <summary>
        /// Brush used as a background for selection highlight
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static readonly DependencyProperty SelectionHighlightBrushProperty =
            DependencyProperty.RegisterAttached(
            "SelectionHighlightBrush",
            typeof(Brush),
            typeof(Highlight),
            new FrameworkPropertyMetadata(null,
                FrameworkPropertyMetadataOptions.Inherits,
                AfterSelectionHighlightBrushChanged));

        static void AfterSelectionHighlightBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RaiseAfterSelectionHighlightBrushChanged(d as UIElement, (Brush)e.NewValue);
        }

        #endregion

        #region After Selection Highlight Brush Changed

        public static readonly RoutedEvent AfterSelectionHighlightBrushChangedEvent =
            EventManager.RegisterRoutedEvent(
            "AfterSelectionHighlightBrushChanged",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(Highlight));

        public static void AddAfterSelectionHighlightBrushChangedHandler(DependencyObject d, RoutedEventHandler handler)
        {
            UIElement uie = d as UIElement;
            if (uie != null)
            {
                uie.AddHandler(AfterSelectionHighlightBrushChangedEvent, handler);
            }
        }
        public static void RemoveAfterSelectionHighlightBrushChangedHandler(DependencyObject d, RoutedEventHandler handler)
        {
            UIElement uie = d as UIElement;
            if (uie != null)
            {
                uie.RemoveHandler(AfterSelectionHighlightBrushChangedEvent, handler);
            }
        }

        static void RaiseAfterSelectionHighlightBrushChanged(UIElement uiElement, Brush newSelectionHighlightBrush)
        {
            if (uiElement == null)
                return;

            RoutedEventArgs newEventArgs = new RoutedEventArgs(AfterSelectionHighlightBrushChangedEvent);
            uiElement.RaiseEvent(newEventArgs);
        }

        #endregion


        #region Hover Highlight Brush

        public static Brush GetHoverHighlightBrush(DependencyObject obj)
        {
            return (Brush)obj.GetValue(HoverHighlightBrushProperty);
        }

        public static void SetHoverHighlightBrush(DependencyObject obj, Brush value)
        {
            obj.SetValue(HoverHighlightBrushProperty, value);
        }

        /// <summary>
        /// Brush used for hover highlight background
        /// </summary>
        public static readonly DependencyProperty HoverHighlightBrushProperty =
            DependencyProperty.RegisterAttached(
            "HoverHighlightBrush",
            typeof(Brush),
            typeof(Highlight),
            new FrameworkPropertyMetadata(null,
                FrameworkPropertyMetadataOptions.Inherits,
                AfterHoverHighlightBrushChanged));

        static void AfterHoverHighlightBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RaiseAfterHoverHighlightBrushChanged(d as UIElement, (Brush)e.NewValue);
        }

        #endregion

        #region After HoverHighlight Brush Changed (EVENT)

        public static readonly RoutedEvent AfterHoverHighlightBrushChangedEvent =
            EventManager.RegisterRoutedEvent(
            "AfterHoverHighlightBrushChanged",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(Highlight));

        public static void AddAfterHoverHighlightBrushChangedHandler(DependencyObject d, RoutedEventHandler handler)
        {

            // todo: check if this and d have common ancestor?
            // if not, then this may be inside popup?

            UIElement uie = d as UIElement;
            if (uie != null)
            {
                uie.AddHandler(AfterHoverHighlightBrushChangedEvent, handler);
            }
        }
        public static void RemoveAfterHoverHighlightBrushChangedHandler(DependencyObject d, RoutedEventHandler handler)
        {
            UIElement uie = d as UIElement;
            if (uie != null)
            {
                uie.RemoveHandler(AfterHoverHighlightBrushChangedEvent, handler);
            }
        }

        static void RaiseAfterHoverHighlightBrushChanged(UIElement uiElement, Brush newHoverHighlightBrush)
        {
            if (uiElement == null)
                return;

            RoutedEventArgs newEventArgs = new RoutedEventArgs(AfterHoverHighlightBrushChangedEvent);
            uiElement.RaiseEvent(newEventArgs);
        }

        #endregion

        #region Foreground Original Value

        static object GetForegroundOriginalValue(DependencyObject obj)
        {
            return (object)obj.GetValue(ForegroundOriginalValueProperty);
        }

        static void SetForegroundOriginalValue(DependencyObject obj, object value)
        {
            obj.SetValue(ForegroundOriginalValueProperty, value);
        }

        static readonly DependencyProperty ForegroundOriginalValueProperty =
            DependencyProperty.RegisterAttached(
            "ForegroundOriginalValue",
            typeof(object),
            typeof(Highlight),
            new PropertyMetadata(null));

        #endregion

        #region Foreground Modified Value

        static object GetForegroundModifiedValue(DependencyObject obj)
        {
            return (object)obj.GetValue(ForegroundModifiedValueProperty);
        }

        static void SetForegroundModifiedValue(DependencyObject obj, object value)
        {
            obj.SetValue(ForegroundModifiedValueProperty, value);
        }

        static readonly DependencyProperty ForegroundModifiedValueProperty =
            DependencyProperty.RegisterAttached(
            "ForegroundModifiedValue",
            typeof(object),
            typeof(Highlight),
            new PropertyMetadata(null));

        #endregion


        #region Text Foreground Responds To Selection (Text Block)

        public static bool GetTextForegroundRespondsToSelection(TextBlock obj)
        {
            return (bool)obj.GetValue(TextForegroundRespondsToSelectionProperty);
        }

        public static void SetTextForegroundRespondsToSelection(TextBlock obj, bool value)
        {
            obj.SetValue(TextForegroundRespondsToSelectionProperty, value);
        }

        public static readonly DependencyProperty TextForegroundRespondsToSelectionProperty =
            DependencyProperty.RegisterAttached(
            "TextForegroundRespondsToSelection",
            typeof(bool),
            typeof(Highlight),
            new PropertyMetadata(false, OnTextForegroundRespondsToSelectionChanged));

        static void OnTextForegroundRespondsToSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tb = d as TextBlock;

            AddAfterHoverHighlightBrushChangedHandler(d, new RoutedEventHandler((_s, _e) =>
            {
                RefreshForegruond(tb);
            }));

            AddAfterSelectionHighlightBrushChangedHandler(d, new RoutedEventHandler((_s, _e) =>
            {
                RefreshForegruond(tb);
            }));
        }

        static void RefreshForegruond(TextBlock tb)
        {
            var selectionHighlightBrush = GetSelectionHighlightBrush(tb) as SolidColorBrush;
            var hoverHighlightBrush = GetHoverHighlightBrush(tb) as SolidColorBrush;

            if (selectionHighlightBrush != null || hoverHighlightBrush != null)
            {
                var foreground_originalValue = GetForegroundOriginalValue(tb);
                var foreground_modifiedValue = GetForegroundModifiedValue(tb);

                var highlight_brush = selectionHighlightBrush;

                if (highlight_brush == null)
                    highlight_brush = hoverHighlightBrush;

                if (highlight_brush == null)
                {
                    // todo: restore original foreground

                    SetSelectionHighlightBrush(tb, null);
                    SetHoverHighlightBrush(tb, null);
                    SetForegroundModifiedValue(tb, null);
                    SetForegroundOriginalValue(tb, null);

                    return;
                }

                var highlight_color = highlight_brush.Color;

                var foreground_brush = tb.Foreground as SolidColorBrush;

                if (foreground_originalValue != null)
                {
                    foreground_brush = foreground_originalValue as SolidColorBrush;
                }

                if (foreground_brush == null)
                {
                    // todo: cleanup
                    // todo: restore original foreground
                    return;
                }

                var foreground_color = foreground_brush.Color;

                //foreground_color.ToContrastingLightnessColor();

                var highlightScRGB = highlight_color.ToScRGBColor();
                var foregroundScRGB = foreground_color.ToScRGBColor();

                var highlightXYZ = KnownColorSpaces.scRGB.ToXYZColor(highlightScRGB);
                var foregroundXYZ = KnownColorSpaces.scRGB.ToXYZColor(foregroundScRGB);

                var highlightLab = (LabColor)KnownColorSpaces.Lab.FromXYZColor(highlightXYZ);
                var foregroundLab = (LabColor)KnownColorSpaces.Lab.FromXYZColor(foregroundXYZ);

                var newForegroundLab = (LabColor)null;

                if (highlightLab.L > 50 && foregroundLab.L > 50)
                {
                    newForegroundLab = new LabColor(foregroundLab.Alpha, 5, foregroundLab.a, foregroundLab.b);
                }
                else if (highlightLab.L < 50 && foregroundLab.L < 50)
                {
                    newForegroundLab = new LabColor(foregroundLab.Alpha, 95, foregroundLab.a, foregroundLab.b);
                }

                if (newForegroundLab == null)
                    return;

                if (foreground_originalValue == null)
                {
                    var binding = BindingOperations.GetBinding(tb, TextBlock.ForegroundProperty);

                    if (binding != null)
                        SetForegroundOriginalValue(tb, binding);
                    else
                        SetForegroundOriginalValue(tb, tb.Foreground);
                }

                var fgBrush = new SolidColorBrush(newForegroundLab.ToWindowsMediaColor());
                fgBrush.Freeze();

                SetForegroundModifiedValue(tb, fgBrush);

                tb.Foreground = fgBrush;
            }
            else
            {
                var originalValue = GetForegroundOriginalValue(tb);

                if (originalValue == null)
                    return;

                if (originalValue is Binding)
                {
                    BindingOperations.SetBinding(tb, TextBlock.ForegroundProperty, originalValue as Binding);
                }
                else if (originalValue != null && object.Equals(GetForegroundModifiedValue(tb), tb.Foreground))
                {
                    tb.SetValue(TextBlock.ForegroundProperty, originalValue);
                }

                SetForegroundModifiedValue(tb, null);
                SetForegroundOriginalValue(tb, null);
            }

        }

        #endregion

    }
}
