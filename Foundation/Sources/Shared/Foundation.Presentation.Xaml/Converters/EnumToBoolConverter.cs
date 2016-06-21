using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using SquaredInfinity.Foundation.Extensions;

namespace SquaredInfinity.Foundation.Presentation.Converters
{
    public class EnumToBoolConverter : MarkupExtension, IValueConverter
    {
        string trueEnumVelue;
        public string TrueEnumValue
        {
            get { return trueEnumVelue; }
            set { trueEnumVelue = value; }
        }

        bool treatAsFlag = false;
        public bool TreatAsFlag
        {
            get { return treatAsFlag; }
            set { treatAsFlag = value; }
        }

        Enum FlagsEnum { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return DependencyProperty.UnsetValue;

            bool result = false;

            if (TreatAsFlag)
            {
                FlagsEnum = value as Enum;

                var trueValue = Enum.Parse(value.GetType(), parameter as string) as Enum;

                result = (value as Enum).HasFlag(trueValue);
            }
            else
            {
                string valueAsString = Enum.GetName(value.GetType(), value);

                if (parameter != null)
                    TrueEnumValue = parameter.ToString();

                if (string.Equals(valueAsString, TrueEnumValue, StringComparison.InvariantCultureIgnoreCase))
                    result = true;
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (TreatAsFlag)
            {
                var enumValue = Enum.Parse(targetType, parameter as string) as Enum;

                if ((bool)value == true)
                {
                    FlagsEnum = FlagsEnum.Set(enumValue);
                    return FlagsEnum;
                }
                else
                {
                    FlagsEnum = FlagsEnum.Unset(enumValue);
                    return FlagsEnum;
                }
            }
            else
            {
                if ((bool)value == false)
                    return DependencyProperty.UnsetValue;

                if (parameter != null)
                    TrueEnumValue = parameter.ToString();

                return Enum.Parse(targetType, TrueEnumValue, true);
            }
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
