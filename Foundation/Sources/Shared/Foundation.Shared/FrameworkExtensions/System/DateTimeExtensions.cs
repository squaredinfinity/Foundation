using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Foundation.Extensions
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

        public static DateTime BeginingOfThisWeek()
        {
            return BeginingOfThisWeek(CultureInfo.CurrentUICulture.DateTimeFormat.FirstDayOfWeek);
        }

        public static DateTime BeginingOfThisWeek(DayOfWeek startOfWeek)
        {
            var now = DateTimeContext.Default.Now;

            int diff = now.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }

            return now.AddDays(-1 * diff).Date;
        }

        public static DateTime BeginingOfMonth(this DateTime me)
        {
            return new DateTime(me.Year, me.Month, 1);
        }

        public static DateTime BeginingOfThisMonth()
        {
            var now = DateTimeContext.Default.Now;

            return new DateTime(now.Year, now.Month, 1);
        }

        public static bool IsToday(this DateTime me)
        {
            return me.Date == DateTimeContext.Default.Now.Date;
        }

        public static bool IsCurrentWeek(this DateTime me)
        {
            return me.Subtract(BeginingOfThisWeek()).TotalDays <= 7;
        }

        public static bool IsCurrentMonth(this DateTime me)
        {
            var meDate = me.Date;
            var todayDate = DateTimeContext.Default.Now.Date;

            return meDate.Year == todayDate.Year
                && meDate.Month == todayDate.Month;
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
    }
}
