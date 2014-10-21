using SquaredInfinity.Foundation.Media.ColorSpaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using SquaredInfinity.Foundation.Extensions;
using System.Windows.Media;

namespace SquaredInfinity.Foundation.Presentation.Converters
{
    public class ColorToGradientBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return DependencyProperty.UnsetValue;

            var color = (System.Windows.Media.Color)value;

            var scrgb = color.ToScRGBColor();

            var xyz = KnownColorSpaces.scRGB.ToXYZColor(scrgb);

            var lab = KnownColorSpaces.Lab.FromXYZColor(xyz) as LabColor;

            var l_base = lab.L;

            var gradientStops = new GradientStopCollection();

            var _lab = new LabColor(0xff, l_base * 1.07, lab.a, lab.b);
            var _c = _lab.ToWindowsMediaColor();

            gradientStops.Add(new GradientStop(_c, 0.5));


            _lab = new LabColor(0xff, l_base * .93, lab.a, lab.b);
            _c = _lab.ToWindowsMediaColor();

            gradientStops.Add(new GradientStop(_c, 1));


            var result = new LinearGradientBrush(gradientStops, 90);

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
