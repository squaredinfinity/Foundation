using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace SquaredInfinity.Presentation.Converters
{
    public enum GetLocalTimeConverterMode
    {
        Local,
        UTC
    }

    public class GetTimeConverter : IValueConverter, IMultiValueConverter
    {
        public GetLocalTimeConverterMode Mode { get; set; }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (Mode == GetLocalTimeConverterMode.Local)
                return DateTime.Now;
            else
                return DateTime.UtcNow;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (Mode == GetLocalTimeConverterMode.Local)
                return DateTime.Now;
            else
                return DateTime.UtcNow;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
