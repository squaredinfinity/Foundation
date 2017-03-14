using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Extensions
{
    public static class ObjectExtensions
    {
        public static string DumpToString(this object obj, string nullValue = "[NULL]", string emptyValue = "[EMPTY]")
        {
            return obj.ToString(valueWhenNull: "[NULL]", valueWhenEmpty: "[EMPTY]");
        }
    }
}
