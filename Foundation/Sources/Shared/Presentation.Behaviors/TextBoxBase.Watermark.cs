using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using SquaredInfinity.Extensions;

namespace SquaredInfinity.Presentation.Behaviors
{
    public class Watermark
    {
        #region Is Visible

        static readonly DependencyProperty IsVisibleProperty =
            DependencyProperty.RegisterAttached(
            "IsVisible",
            typeof(bool),
            typeof(Watermark),
            new PropertyMetadata(false));

        static void SetIsVisible(TextBoxBase element, bool value)
        {
            element.SetValue(IsVisibleProperty, value);
        }

        static bool GetIsVisible(TextBoxBase element)
        {
            return (bool)element.GetValue(IsVisibleProperty);
        }

        #endregion

        #region Text

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.RegisterAttached(
            "Text",
            typeof(string),
            typeof(Watermark),
            new PropertyMetadata("", OnWatermarkTextChanged));

        public static void SetText(TextBoxBase element, string value)
        {
            element.SetValue(TextProperty, value);
        }

        public static string GetText(TextBoxBase element)
        {
            return (string)element.GetValue(TextProperty);
        }

        #endregion

        #region Watermark Events

        public static void OnWatermarkTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tb = d as TextBoxBase;

            tb.GotFocus -= Watermarked_GotFocus;
            tb.GotFocus += Watermarked_GotFocus;

            tb.LostFocus -= Watermarked_LostFocus;
            tb.LostFocus += Watermarked_LostFocus;

            tb.TextChanged -= Watermarked_TextChanged;
            tb.TextChanged += Watermarked_TextChanged;

            UpdateWatermarkVisibility(tb);
        }

        static void Watermarked_LostFocus(object sender, RoutedEventArgs e)
        {
            var tb = sender as TextBoxBase;
            UpdateWatermarkVisibility(tb);
        }

        static void Watermarked_TextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = sender as TextBoxBase;
            UpdateWatermarkVisibility(tb);
        }

        static void Watermarked_GotFocus(object sender, RoutedEventArgs e)
        {
            var tb = sender as TextBoxBase;
            UpdateWatermarkVisibility(tb);
        }

        static void UpdateWatermarkVisibility(TextBoxBase tb)
        {
            // Never show when focused
            if (tb.IsFocused || tb.IsKeyboardFocusWithin)
            {
                SetIsVisible(tb, false);
                return;
            }

            var textBox = tb as TextBox;
            if (textBox != null)
            {
                // Only show when text is empty
                SetIsVisible(tb, textBox.Text.IsNullOrEmpty());
                return;
            }
        }

        #endregion
    }
}
