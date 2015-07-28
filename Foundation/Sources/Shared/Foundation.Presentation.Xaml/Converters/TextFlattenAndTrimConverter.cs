using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using SquaredInfinity.Foundation;
using SquaredInfinity.Foundation.Extensions;

namespace SquaredInfinity.Foundation.Presentation.Converters
{
    public class TextFlattenAndTrimConverter : IValueConverter
    {
        public int? MaxLength { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string txt = value as string;

            if (txt == null)
                return DependencyProperty.UnsetValue;

            txt =
                txt.TrimEachLine(removeEmptyLines: true)
                .Replace(Environment.NewLine, " ");

            if (MaxLength.HasValue && MaxLength.Value > 0)
            {
                if (txt.Length > MaxLength.Value)
                {
                    txt = txt.Substring(0, MaxLength.Value - 4) + " ...";
                }
            }

            return txt;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
