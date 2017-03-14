using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SquaredInfinity.Extensions
{
    public static class StringExtensions
    {
        public static string ToTitleCase(this string str)
        {
            return str.ToTitleCase(CultureInfo.CurrentUICulture);
        }

        public static string ToTitleCase(this string str, CultureInfo cultureInfo)
        {
            return cultureInfo.TextInfo.ToTitleCase(str);
        }
    }
}
