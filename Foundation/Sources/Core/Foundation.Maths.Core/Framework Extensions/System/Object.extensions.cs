using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Extensions
{
    public static class ObjectExtensions
    {
        public static string ToStringWithNullOrEmpty(this object obj)
        {
            return obj.ToString(valueWhenNull: "[NULL]", valueWhenEmpty:"[EMPTY]");
        }

        public static string ToString(this object obj, string valueWhenNull)
        {
            if (obj == null)
                return valueWhenNull;

            return obj.ToString();
        }

        public static string ToString(this object obj, string valueWhenNull, string valueWhenEmpty)
        {
            if (obj == null)
                return valueWhenNull;

            var toString = obj.ToString();

            if (string.IsNullOrEmpty(toString))
                return valueWhenEmpty;

            return toString;
        }
    }
}
