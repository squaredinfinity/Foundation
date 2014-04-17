using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Foundation.Extensions
{
    public static class Int32Extensions
    {
        public static bool IsFlagSet(this Int32 number, Int32 flag)
        {
            return (number & flag) == flag;
        }
    }
}
