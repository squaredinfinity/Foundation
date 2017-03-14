using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace SquaredInfinity.Presentation.Converters
{
    public class CompareToBoolConverter : IValueConverter
    {
        object _comparandValue;
        public object ComparandValue
        {
            get { return _comparandValue; }
            set { _comparandValue = value; }
        }

        CompareToBoolConverterMode _mode = CompareToBoolConverterMode.Equals;
        public CompareToBoolConverterMode Mode
        {
            get { return _mode; }
            set { _mode = value; }
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null && ComparandValue == null)
            {
                if (Mode == CompareToBoolConverterMode.Equals)
                    return true;
                else
                    return false;
            }

            var valueType = value.GetType();
            var expectedValueType = ComparandValue.GetType();

            var comparand = ComparandValue;

            if (!object.Equals(valueType, expectedValueType))
            {
                try
                {
                    comparand = System.Convert.ChangeType(ComparandValue, valueType);
                }
                catch
                {
                    // unable to change type
                    // todo: log
                    return false;
                }
            }

            switch (Mode)
            {
                case CompareToBoolConverterMode.Equals:
                    return DoEquals(value, comparand);
                case CompareToBoolConverterMode.NotEquals:
                    return !DoEquals(value, comparand);
                case CompareToBoolConverterMode.GreaterThan:
                    return DoGreaterThan(value, comparand);
                case CompareToBoolConverterMode.GreaterOrEqual:
                    return DoEquals(value, comparand) || DoGreaterThan(value, comparand);
                case CompareToBoolConverterMode.LessThan:
                    return DoLessThan(value, comparand);
                case CompareToBoolConverterMode.LessOrEqual:
                    return DoEquals(value, comparand) || DoLessThan(value, comparand);
                default:
                    {
                        // todo: log
                        return DependencyProperty.UnsetValue;
                    }
            }
        }


        bool DoGreaterThan(object value, object comparand)
        {
            var comparable_value = value as IComparable;

            if (comparable_value == null)
                return false; // cannot compare values

            return comparable_value.CompareTo(comparand) == 1;
        }

        bool DoLessThan(object value, object comparand)
        {
            var comparable_value = value as IComparable;

            if (comparable_value == null)
                return false; // cannot compare values

            return comparable_value.CompareTo(comparand) == -1;
        }

        bool DoEquals(object value, object comparand)
        {
            if (object.Equals(value, ComparandValue))
                return true;
            else
                return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public enum CompareToBoolConverterMode
        {
            Equals,
            NotEquals,
            GreaterThan,
            GreaterOrEqual,
            LessThan,
            LessOrEqual
        }
    }
}
