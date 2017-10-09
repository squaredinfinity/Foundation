using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Extensions
{
    public static class BooleanExtensions
    {
        public static string ToString(this bool booleanValue, string whenTrue, string whenFalse)
        {
            if (booleanValue)
                return whenTrue;
            else
                return whenFalse;
        }
    }
}
