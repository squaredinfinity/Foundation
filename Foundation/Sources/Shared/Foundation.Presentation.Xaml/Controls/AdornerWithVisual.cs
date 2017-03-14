using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace SquaredInfinity.Presentation.Controls
{
    public class AdornerWithVisual : Adorner
    {
        VisualCollection VisualChildren { get; set; }

        #region Adorner Visual

        public FrameworkElement AdornerVisual
        {
            get { return (FrameworkElement)GetValue(AdornerVisualProperty); }
            set { SetValue(AdornerVisualProperty, value); }
        }

        public static readonly DependencyProperty AdornerVisualProperty =
            DependencyProperty.Register(
            "AdornerVisual",
            typeof(FrameworkElement),
            typeof(AdornerWithVisual),
            new FrameworkPropertyMetadata(OnAdornerVisualChanged));

        static void OnAdornerVisualChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var adorner = d as AdornerWithVisual;

            var oldElement = e.OldValue as FrameworkElement;

            if (oldElement != null)
            {
                adorner.VisualChildren.Remove(oldElement);
            }

            var newElement = e.NewValue as FrameworkElement;

            if (newElement != null)
            {
                adorner.VisualChildren.Add(newElement);
            }
        }

        #endregion

        public AdornerWithVisual(UIElement adornedElement)
            : base(adornedElement)
        {
            VisualChildren = new VisualCollection(this);
            Focusable = false;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return VisualChildren.Count;
            }
        }

        protected override Visual GetVisualChild(int index)
        {
            return VisualChildren[index];
        }

        protected override Size MeasureOverride(Size constraint)
        {
            return base.MeasureOverride(constraint);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var desiredWidth = AdornedElement.DesiredSize.Width;
            var desiredHeight = AdornedElement.DesiredSize.Height;

            var adornerWidth = this.DesiredSize.Width;
            var adornerHeight = this.DesiredSize.Height;

            AdornerVisual.Arrange(new Rect(AdornedElement.DesiredSize));

            return finalSize;
        }
    }
}
