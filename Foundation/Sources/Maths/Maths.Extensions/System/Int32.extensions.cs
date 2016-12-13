using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Extensions
{
    public static class Int32Extensions
    {
        public static Int32 FindOddNumber(this Int32 search_origin, ForwardBackwardDirection direction)
        {
            var r = 0;

            search_origin.DivRem(2, out r);

            if (r == 1 || r == -1)
                return search_origin;

            if (direction == ForwardBackwardDirection.Forward)
            {
                return search_origin + 1;
            }
            else
            {
                return search_origin - 1;
            }

        }

        public static Int32 FindEvenNumber(this Int32 search_origin, ForwardBackwardDirection direction)
        {
            var r = 0;

            search_origin.DivRem(2, out r);

            if (r == 0)
                return search_origin;

            if (direction == ForwardBackwardDirection.Forward)
                return search_origin + 1;
            else
                return search_origin - 1;

        }

        public static Int32 FindDivisorWithoutReminder(this Int32 search_origin, Int32 dividend, ForwardBackwardDirection direction)
        {
            var r = 0;

            dividend.DivRem(search_origin, out r);

            if (r == 0)
                return search_origin;

            if (direction == ForwardBackwardDirection.Forward)
                return FindNextDivisorWithoutReminder(dividend, search_origin);
            else
                return FindPreviousDivisorWithoutReminder(dividend, search_origin);
        }

        public static Int32 FindPreviousDivisorWithoutReminder(this Int32 search_origin, Int32 dividend)
        {
            var r = -1;

            while (r != 0)
            {
                dividend.DivRem(--search_origin, out r);
            }

            return search_origin;
        }

        public static Int32 FindNextDivisorWithoutReminder(Int32 search_origin, Int32 dividend)
        {
            var r = -1;

            while (r != 0)
            {
                dividend.DivRem(++search_origin, out r);
            }

            return search_origin;
        }

        public static Int32 DivRem(this Int32 a, Int32 b, out Int32 result)
        {
            result = a % b;

            return a / b;
        }

        /// <summary>
        /// Returns digit at specified position.
        /// </summary>
        /// <param name="number"></param>
        /// <param name="position">the position of digit to return. units position is 0, tens position is 1, hunderets position is 2 etc.</param>
        /// <returns></returns>
        public static int DigitAt(this Int32 number, uint position)
        {
            return (number / (int)Math.Pow(10, position)) % 10;
        }
    }
}
