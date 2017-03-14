using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Types.Mapping.ValueConverting
{
    public class PreserveOriginalValueConverter : IValueConverter<object, object>
    {
        public static readonly IValueConverter Instance = new PreserveOriginalValueConverter();

        Type TYPE_Object = typeof(object);

        public object Convert(object source)
        {
            return source;
        }

        public Type From
        {
            get { return TYPE_Object; }
        }

        public Type To
        {
            get { return TYPE_Object; }
        }
    }
}
