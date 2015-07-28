using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SquaredInfinity.Foundation.Extensions;

namespace SquaredInfinity.Foundation.Presentation.Controls
{
    public class Canvas : System.Windows.Controls.Canvas
    {
        protected override Size MeasureOverride(Size constraint)
        {
            // allow children to measure themeselves
            foreach(var child in InternalChildren.OfType<UIElement>())
            {
                child.Measure(constraint);
            }

            double width =
                (from c in InternalChildren.OfType<UIElement>()
                 select c.DesiredSize.Width + Canvas.GetLeft(c).GetValueOrDefaultIfInfinityOrNaN(0))
                 .DefaultIfEmpty(0)
                 .Max();

            double height = 
                (from c in InternalChildren.OfType<UIElement>()
                 select c.DesiredSize.Height + Canvas.GetTop(c).GetValueOrDefaultIfInfinityOrNaN(0))
                 .DefaultIfEmpty(0)
                 .Max();

            if (double.IsNaN(width))
                width = MinWidth;

            if (double.IsNaN(height))
                height = MinHeight;

            return new Size(width, height);
        }

        protected override void OnChildDesiredSizeChanged(UIElement child)
        {
            base.OnChildDesiredSizeChanged(child);

            this.SetValue(FrameworkElement.HeightProperty, DependencyProperty.UnsetValue);
            this.SetValue(FrameworkElement.WidthProperty, DependencyProperty.UnsetValue);
            this.InvalidateMeasure();
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);            
        }
    }
}
