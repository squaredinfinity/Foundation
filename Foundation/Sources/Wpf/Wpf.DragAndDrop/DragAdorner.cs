using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Media;

namespace SquaredInfinity.Foundation.Presentation.DragDrop
{
    class DragAdorner : Adorner
    {
        readonly AdornerLayer AdornerLayer;
        readonly UIElement Adornment;

        public DragAdorner(UIElement adornedElement, UIElement adornment)
            : base(adornedElement)
        {
            AdornerLayer = AdornerLayer.GetAdornerLayer(adornedElement);
            AdornerLayer.Add(this);
            Adornment = adornment;
            IsHitTestVisible = false;
        }

        Point _mousePosition;
        public Point MousePosition
        {
            get { return _mousePosition; }
            set
            {
                if (_mousePosition != value)
                {
                    _mousePosition = value;

                    AdornerLayer.Update(AdornedElement);
                }
            }
        }

        public void Detatch()
        {
            this.AdornerLayer.Remove(this);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Adornment.Arrange(new Rect(finalSize));

            return finalSize;
        }

        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            var result = new GeneralTransformGroup();

            result.Children.Add(base.GetDesiredTransform(transform));

            var mousePositionTransform = new TranslateTransform(MousePosition.X, MousePosition.Y);
            result.Children.Add(mousePositionTransform);

            var mousePositionOffsetTransform = new TranslateTransform(-4, -4);
            result.Children.Add(mousePositionOffsetTransform);

            return result;
        }

        protected override Visual GetVisualChild(int index)
        {
            return Adornment;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            Adornment.Measure(constraint);
            return Adornment.DesiredSize;
        }

        protected override int VisualChildrenCount
        {
            get { return 1; }
        }
    }
}