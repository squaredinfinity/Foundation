using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Extensions
{
    public static class DateTimeExtensions
    {
        public static IDateTimeContext ToDateTimeContext(this DateTime dt)
        {
            return new DateTimeContext(() => dt);
        }
    }
}
