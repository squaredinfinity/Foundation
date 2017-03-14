using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Extensions
{
    /// <summary>
    /// Extensions of a Decimal type.
    /// </summary>
    public static partial class DecimalExtensions
    {
        public static string ToStringNoTrailingZeros(this decimal d)
        {
            return (d / 1.000000000000000000000000000000000m).ToString();
        }

        
    }
}
