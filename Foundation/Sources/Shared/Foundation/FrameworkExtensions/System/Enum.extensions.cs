using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Foundation.Extensions
{
    /// <summary>
    /// Extensions for Enum types.
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Determines whether [is flag set].
        /// </summary>
        /// <param name="me">The enum.</param>
        /// <param name="bitFlag">The bit flag.</param>
        /// <returns>
        /// 	<c>true</c> if [is flag set]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsFlagSet(this Enum me, Enum bitFlag)
        {
            if (me.GetType() != bitFlag.GetType())
                throw new ArgumentException("Types of parameters have to be the same");

            return
                (me as IConvertible).ToInt32(null).IsFlagSet((bitFlag as IConvertible).ToInt32(null));
        }
    }
}
