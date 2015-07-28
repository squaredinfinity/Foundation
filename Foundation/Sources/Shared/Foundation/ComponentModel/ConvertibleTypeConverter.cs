using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.ComponentModel
{
    public class ConvertibleTypeConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            var typeConvertible = context.Instance as IConvertibleType;

            return typeConvertible.CanConvertTo(destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            var typeConvertible = value as IConvertibleType;

            return typeConvertible.ConvertTo(destinationType);
        }
    }
}
