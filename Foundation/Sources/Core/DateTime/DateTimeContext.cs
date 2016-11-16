using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Foundation
{
    /// <summary>
    /// Provides a DateTime context.
    /// Should be used instead of DateTime.Now for unit testing purposes.
    /// </summary>
    public class DateTimeContext : IDateTimeContext
    {
        Func<DateTime> FuncNow = null;

        /// <summary>
        /// Gets the current DateTime
        /// </summary>
        /// <value>The now.</value>
        public DateTime Now
        {
            get { return FuncNow(); }
        }

        static IDateTimeContext defaultContext = new DateTimeContext();

        /// <summary>
        /// Gets the default DateTime context.
        /// This should be used instead of DateTime.Now for unit testing purposes.
        /// </summary>
        /// <value>The default.</value>
        public static IDateTimeContext Default
        {
            get { return defaultContext; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeContext"/> class.
        /// Current time will always result same value as DateTime.Now.
        /// </summary>
        public DateTimeContext()
            : this(() => DateTime.Now)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeContext"/> class.
        /// Current time can be calculated in a function.
        /// For example, to 'move seven days back in time' we can call: 
        /// new DateTimeContext(() => return DateTime.Now.Subtract(TimeSpan.FromDays(7)));
        /// </summary>
        /// <param name="now">The now.</param>
        public DateTimeContext(Func<DateTime> now)
        {
            this.FuncNow = now;
        }

        /// <summary>
        /// Sets the DateTimeContext to a static value represented by the string.
        /// e.g. DateTimeContext.FromString("06/13/2008 01:01:01.0");
        /// NOTE: InvariantCulture will be used, so date format is MM/dd/yyyy
        /// </summary>
        /// <param name="dateTimeString">The date time string.</param>
        /// <returns></returns>
        public static IDateTimeContext FromString(string dateTimeString)
        {
            return FromString(dateTimeString, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Sets the DateTimeContext to a static value represented by the string.
        /// </summary>
        /// <param name="dateTimeString">The date time string.</param>
        /// <param name="cultureInfo">The culture info.</param>
        /// <returns></returns>
        public static IDateTimeContext FromString(string dateTimeString, CultureInfo cultureInfo)
        {
            return new DateTimeContext(() => DateTime.Parse(dateTimeString, cultureInfo));
        }

        /// <summary>
        /// Sets the default context.
        /// WARNING: This will affect ALL code that uses default context and should be used for UnitTesting ONLY!
        /// </summary>
        /// <param name="newDefaultContext">The new default context.</param>
        public static void SetDefaultContext(IDateTimeContext newDefaultContext)
        {
            defaultContext = newDefaultContext;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // just a placeholder to be able to use it in a using statements.
        }

        public DateTime BeginingOfThisWeek(DayOfWeek startOfWeek)
        {
            var now = Now;

            int diff = now.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }

            return now.AddDays(-1 * diff).Date;
        }

        public DateTime BeginingOfThisWeek()
        {
            return BeginingOfThisWeek(CultureInfo.CurrentUICulture.DateTimeFormat.FirstDayOfWeek);
        }

        public DateTime BeginingOfThisMonth()
        {
            var now = Now;

            return new DateTime(now.Year, now.Month, 1);
        }

        public bool IsToday(DateTime dt)
        {
            return dt.Date == Now.Date;
        }

        public bool IsCurrentWeek(DateTime dt)
        {
            return dt.Subtract(BeginingOfThisWeek()).TotalDays <= 7;
        }

        public bool IsCurrentMonth(DateTime dt)
        {
            var meDate = dt.Date;
            var todayDate = Now.Date;

            return meDate.Year == todayDate.Year
                && meDate.Month == todayDate.Month;
        }
    }
}
