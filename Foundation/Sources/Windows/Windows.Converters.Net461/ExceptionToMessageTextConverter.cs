﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace SquaredInfinity.Windows.Converters
{
    public class ExceptionToMessageTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var aggregate_exception = value as AggregateException;

            if (aggregate_exception != null)
            {
                var sb = new StringBuilder();

                sb.AppendLine(aggregate_exception.Message);

                foreach (var ex in aggregate_exception.InnerExceptions)
                {
                    sb.AppendLine(ex.Message);
                }

                return sb.ToString();
            }

            var exception = value as Exception;
            if (exception != null)
                return exception.Message;

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}