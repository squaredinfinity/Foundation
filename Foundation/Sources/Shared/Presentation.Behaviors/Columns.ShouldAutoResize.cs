using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using SquaredInfinity.Extensions;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace SquaredInfinity.Presentation.Behaviors
{
    public static partial class Columns
    {
        #region Should Auto Resize

        public static readonly DependencyProperty ShouldAutoResizeProperty =
            DependencyProperty.RegisterAttached(
            "ShouldAutoResize",
            typeof(bool),
            typeof(Columns),
            new PropertyMetadata(OnAutoResizeChangedChanged));


        public static void SetShouldAutoResize(System.Windows.Controls.ListView element, bool value)
        {
            element.SetValue(ShouldAutoResizeProperty, value);
        }

        public static bool GetShouldAutoResize(System.Windows.Controls.ListView element)
        {
            return (bool)element.GetValue(ShouldAutoResizeProperty);
        }

        #endregion

        #region INTERNAL ItemContainerGenerator ItemsChanged Handler

        static readonly DependencyProperty INTERNAL_ItemContainerGeneratorEventHandlerProperty =
            DependencyProperty.RegisterAttached(
            "INTERNAL_ItemContainerGeneratorEventHandlerProperty",
            typeof(ItemContainerGenerator_ItemsChangedHander),
            typeof(Columns), new PropertyMetadata());

        static void SetINTERNAL_ItemContainerGeneratorEventHandler(
            System.Windows.Controls.ListView element,
            ItemContainerGenerator_ItemsChangedHander value)
        {
            element.SetValue(INTERNAL_ItemContainerGeneratorEventHandlerProperty, value);
        }

        static ItemContainerGenerator_ItemsChangedHander GetINTERNAL_ItemContainerGeneratorEventHandler(System.Windows.Controls.ListView element)
        {
            return (ItemContainerGenerator_ItemsChangedHander)element.GetValue(INTERNAL_ItemContainerGeneratorEventHandlerProperty);
        }

        #endregion

        static void OnAutoResizeChangedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var lv = d as ListView;

            if (lv == null)
                return;

            if ((bool)e.NewValue)
            {
                var handler = new ItemContainerGenerator_ItemsChangedHander(lv, (_lv) =>
                {
                    if (_lv == null)
                        return;

                    var gv = _lv.View as GridView;

                    if (gv == null)
                        return;

                    foreach (var col in gv.Columns)
                    {
                        if (double.IsNaN(col.Width))
                            col.Width = col.ActualWidth;

                        col.Width = double.NaN;
                    }
                });

                SetINTERNAL_ItemContainerGeneratorEventHandler(lv, handler);
            }
            else
            {
                var handler = GetINTERNAL_ItemContainerGeneratorEventHandler(lv);
                if (handler != null)
                    handler.Dispose();
            }
        }
    }
}
