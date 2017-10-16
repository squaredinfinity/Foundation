using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime UnixEpochBegining()
        {
            return new DateTime(1970, 1, 1);
        }

        public static DateTime BeginingOfWeek(this DateTime dt)
        {
            return BeginingOfWeek(dt, CultureInfo.CurrentUICulture.DateTimeFormat.FirstDayOfWeek);
        }

        public static DateTime BeginingOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = dt.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }

            return dt.AddDays(-1 * diff).Date;
        }

        public static DateTime BeginingOfMonth(this DateTime me)
        {
            return new DateTime(me.Year, me.Month, 1);
        }

        /// <summary>
        /// DateTime.ToString("dd/MM/yyyy") will return date in different format depending on current culture:
        ///     EN: 23/11/2011
        ///     DE: 23.11.2011
        ///     CN: 23-11-2011
        /// 
        /// This method will automatically use invariant culture to return date in format of dd/MM/yyyy
        /// i.e. not replacing '/' with a culture specific seprator
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ToInvariantDateString(this DateTime dt)
        {
            return dt.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns elapsed time between specified DateTime and Now.
        /// Takes into account Kind of specified Date Time (e.g. Local vs UTC)
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static TimeSpan ElapsedToNow(this DateTime dt)
        {
            if (dt.Kind == DateTimeKind.Local)
                return DateTime.Now - dt;
            else return DateTime.UtcNow - dt;
        }
    }
}
