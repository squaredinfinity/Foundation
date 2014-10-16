using SquaredInfinity.Foundation.Presentation.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace SquaredInfinity.Foundation.Presentation.Converters
{
    public class GetApplicationResourceConverter : IValueConverter
    {
        public GetApplicationResourceConverterMode Mode { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (Mode)
            {
                case GetApplicationResourceConverterMode.LoadImageFromMainAssembly:
                    return ResourcesManager.LoadImageFromEntryAssembly(value as string);
                default:
                    // todo
                    //Logger.LogUnexpectedEnumValue(Mode);
                    break;
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public enum GetApplicationResourceConverterMode
    {
        Default,
        LoadImageFromMainAssembly = Default
    }
}
