using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Foundation.Extensions
{
    public static class Int64Extensions
    {
        public static bool IsFlagSet(this Int64 number, Int16 flag)
        {
            return (number & flag) == flag;
        }

        public static bool IsFlagSet(this Int64 number, Int32 flag)
        {
            return (number & flag) == flag;
        }

        public static bool IsFlagSet(this Int64 number, Int64 flag)
        {
            return (number & flag) == flag;
        }
    }
}
