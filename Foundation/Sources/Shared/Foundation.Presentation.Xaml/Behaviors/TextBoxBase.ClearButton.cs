using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using SquaredInfinity.Foundation.Extensions;
using System.Windows.Input;

namespace SquaredInfinity.Foundation.Presentation.Behaviors
{
    public class ClearButton
    {
        #region Is Visible

        public static readonly DependencyProperty IsVisibleProperty = 
            DependencyProperty.RegisterAttached(
            "IsVisible", 
            typeof(bool), 
            typeof(ClearButton), 
            new PropertyMetadata(false, OnIsVisibleChanged));

        public static void SetIsVisible(TextBoxBase element, bool value)
        {
            element.SetValue(IsVisibleProperty, value);
        }

        public static bool GetIsVisible(TextBoxBase element)
        {
            return (bool)element.GetValue(IsVisibleProperty);
        }

        #endregion

        #region Intercepts Esc Key

        public static readonly DependencyProperty InterceptsEscKeyProperty =
            DependencyProperty.RegisterAttached(
            "InterceptsEscKey",
            typeof(bool),
            typeof(ClearButton),
            new PropertyMetadata(false, OnSetInterceptsEscKeyChanged));

        public static void SetInterceptsEscKey(TextBoxBase element, bool value)
        {
            element.SetValue(InterceptsEscKeyProperty, value);
        }

        public static bool GetSetInterceptsEscKey(TextBoxBase element)
        {
            return (bool)element.GetValue(InterceptsEscKeyProperty);
        }

        static void OnSetInterceptsEscKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tb = d as TextBoxBase;

            // NOTE: it has to happen on key down, because this is the event used by windows (e.g. when dismissing dialog box, it happens on ESC key down, no key up)

            if((bool) e.NewValue)
            {
                tb.PreviewKeyDown -= tb_PreviewKeyDown;
                tb.PreviewKeyDown += tb_PreviewKeyDown;
            }
            else
            {
                tb.PreviewKeyDown -= tb_PreviewKeyDown;
            }
        }

        static void tb_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key != Key.Escape)
                return;

            var tbb = sender as TextBoxBase;

            var tb = tbb as TextBox;

            if (tb == null)
                return;

            if (tb.Text.IsNullOrEmpty())
                return;            

            ClearTextBox(tb);

            e.Handled = true;
        }

        #endregion

        #region Clear Button Events

        static void OnIsVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tb = d as TextBoxBase;

            var template = tb.Template;

            if (template == null)
            {
                if ((bool)e.NewValue == true)
                {
                    tb.Loaded += ShowClearButton_Loaded;
                }
                else
                {
                    tb.Loaded -= ShowClearButton_Loaded;
                }
            }
            else
            {
                InitialiseClearButton(tb);
            }
        }

        static void InitialiseClearButton(TextBoxBase tb)
        {
            bool isEnabled = GetIsVisible(tb);

            var template = tb.Template;

            var clearButton = template.FindName("PART_ClearButton", tb) as ButtonBase;

            if (clearButton == null)
                return;

            if (isEnabled)
            {
                clearButton.Click += ShowClearButton_Click;
                tb.TextChanged += ShowClearButton_TextChanged;
            }
            else
            {
                clearButton.Click -= ShowClearButton_Click;
                tb.TextChanged -= ShowClearButton_TextChanged;
            }

            UpdateClearButtonVisibility(tb);
        }

        static void ShowClearButton_Loaded(object sender, RoutedEventArgs e)
        {
            InitialiseClearButton(sender as TextBoxBase);
        }

        static void ShowClearButton_TextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = sender as TextBoxBase;
            UpdateClearButtonVisibility(tb);
        }

        static void UpdateClearButtonVisibility(TextBoxBase tb)
        {
            var template = tb.Template;
            var clearButton = template.FindName("PART_ClearButton", tb) as ButtonBase;

            if (clearButton == null)
                return;

            var textBox = tb as TextBox;
            if (textBox != null)
            {
                // only show clear button when there is text

                if (textBox.Text.IsNullOrEmpty())
                {
                    clearButton.Visibility = Visibility.Collapsed;
                }
                else
                {
                    clearButton.Visibility = Visibility.Visible;
                }
            }
        }

        static void ShowClearButton_Click(object sender, RoutedEventArgs e)
        {
            var b = sender as ButtonBase;

            var parent = b.FindVisualParent<TextBoxBase>();

            if (parent == null)
                return;

            ClearTextBox(parent);
        }

        static void ClearTextBox(TextBoxBase textBoxBase)
        {
            var textBox = textBoxBase as TextBox;

            if (textBox == null)
                return;

            textBox.Text = string.Empty;
            return;
        }

        #endregion
    }
}
