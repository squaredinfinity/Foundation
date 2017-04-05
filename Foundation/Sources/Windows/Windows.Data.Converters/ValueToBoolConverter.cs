using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SquaredInfinity.Presentation.Converters
{
    public class ValueToBoolConverter : IValueConverter
    {
        object _expectedValue;
        public object ExpectedValue
        {
            get { return _expectedValue; }
            set { _expectedValue = value; }
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (object.Equals(value, ExpectedValue))
                return true;

            if (value == null || ExpectedValue == null)
                return false;

            var valueType = value.GetType();

            var expectedValueType = ExpectedValue.GetType();

            if (object.Equals(valueType, expectedValueType))
                return false;

            var convertedExpectedValue = System.Convert.ChangeType(ExpectedValue, valueType);

            return object.Equals(value, convertedExpectedValue);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
