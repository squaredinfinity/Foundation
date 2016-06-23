using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Determines whether the specified string [is null or empty].
        /// </summary>
        /// <param name="str">input string</param>
        /// <returns>
        /// 	<c>true</c> if the specified string [is null or empty]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullOrEmpty(this string str, bool treatWhitespaceOnlyStringAsEmpty = true)
        {
            if (treatWhitespaceOnlyStringAsEmpty)
                return string.IsNullOrWhiteSpace(str);
            else
                return string.IsNullOrEmpty(str);
        }
    }
}
