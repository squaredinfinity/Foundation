using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SquaredInfinity.Extensions;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows;

namespace SquaredInfinity.Presentation.Converters
{
    [ContentProperty("Brushes")]
    public class IndexToBrushConverter : MarkupExtension, IValueConverter
    {
        List<Brush> _brushes = new List<Brush>();
        public List<Brush> Brushes
        {
            get { return _brushes; }
            set { _brushes = value; }
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (Brushes.IsNullOrEmpty())
                return DependencyProperty.UnsetValue;

            if(value == null)
                return DependencyProperty.UnsetValue;

            var i = System.Convert.ToInt32(value);

            return Brushes[i % Brushes.Count];
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
