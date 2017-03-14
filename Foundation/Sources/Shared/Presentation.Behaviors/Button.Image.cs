using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace SquaredInfinity.Presentation.Behaviors
{
    public static partial class ButtonBehaviors
    {
        #region Image

        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.RegisterAttached(
            "Image",
            typeof(ImageSource),
            typeof(ButtonBehaviors),
            new PropertyMetadata(null));

        public static void SetImage(System.Windows.Controls.Button element, ImageSource value)
        {
            element.SetValue(ImageProperty, value);
        }

        public static ImageSource GetImage(System.Windows.Controls.Button element)
        {
            return (ImageSource)element.GetValue(ImageProperty);
        }

        #endregion

        #region Image Width

        public static readonly DependencyProperty ImageWidthProperty =
            DependencyProperty.RegisterAttached(
            "ImageWidth",
            typeof(double),
            typeof(ButtonBehaviors),
            new PropertyMetadata(null));

        public static void SetImageWidth(System.Windows.Controls.Button element, double value)
        {
            element.SetValue(ImageWidthProperty, value);
        }

        public static double GetImageWidth(System.Windows.Controls.Button element)
        {
            return (double)element.GetValue(ImageWidthProperty);
        }

        #endregion

        #region Image Height

        public static readonly DependencyProperty ImageHeightProperty =
            DependencyProperty.RegisterAttached(
            "ImageHeight",
            typeof(double),
            typeof(ButtonBehaviors),
            new PropertyMetadata(null));

        public static void SetImageHeight(System.Windows.Controls.Button element, double value)
        {
            element.SetValue(ImageHeightProperty, value);
        }

        public static double GetImageHeight(System.Windows.Controls.Button element)
        {
            return (double)element.GetValue(ImageHeightProperty);
        }

        #endregion

       

    }
}
