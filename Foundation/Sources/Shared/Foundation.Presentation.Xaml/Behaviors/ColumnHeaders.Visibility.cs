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
    public class ColumnHeaders
    {
        #region Visibility

        public static readonly DependencyProperty IsVisibleProperty =
            DependencyProperty.RegisterAttached(
            "Visibility",
            typeof(Visibility),
            typeof(ColumnHeaders),
            new PropertyMetadata(Visibility.Visible, OnVisibilityChanged));

        public static void SetVisibility(ListView element, Visibility value)
        {
            element.SetValue(IsVisibleProperty, value);
        }

        public static Visibility GetVisibility(ListView element)
        {
            return (Visibility)element.GetValue(IsVisibleProperty);
        }

        static void OnVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var lv = d as ListView;

            if (!lv.IsLoaded)
            {
                lv.Loaded += lv_Loaded;
            }
            else
            {
                UpdateGridViewHeaderRowPresenterVisibility(lv);
            }
        }

        static void lv_Loaded(object sender, RoutedEventArgs e)
        {
            var lv = sender as ListView;
            lv.Loaded -= lv_Loaded;

            UpdateGridViewHeaderRowPresenterVisibility(lv);
        }

        static void UpdateGridViewHeaderRowPresenterVisibility(ListView lv)
        {
            var header = lv.FindVisualDescendant<GridViewHeaderRowPresenter>();
            if(header != null)
            {
                header.Visibility = GetVisibility(lv);
            }
        }

        #endregion
    }
}
