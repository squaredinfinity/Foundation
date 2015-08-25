using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.Foundation
{
    public static class MathExtensions
    {
        public static int FindOddNumber(int search_origin, ForwardBackwardDirection direction)
        {
            var r = 0;

            Math.DivRem(search_origin, 2, out r);

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

        public static int FindEvenNumber(int search_origin, ForwardBackwardDirection direction)
        {
            var r = 0;

            Math.DivRem(search_origin, 2, out r);

            if (r == 0)
                return search_origin;

            if (direction == ForwardBackwardDirection.Forward)
                return search_origin + 1;
            else
                return search_origin - 1;

        }

        public static int FindDivisorWithoutReminder(int dividend, int search_origin, ForwardBackwardDirection direction)
        {
            var r = 0;

            Math.DivRem(dividend, search_origin, out r);

            if (r == 0)
                return search_origin;

            if (direction == ForwardBackwardDirection.Forward)
                return FindNextDivisorWithoutReminder(dividend, search_origin);
            else
                return FindPreviousDivisorWithoutReminder(dividend, search_origin);
        }

        public static int FindPreviousDivisorWithoutReminder(int dividend, int search_origin)
        {
            var r = -1;

            while (r != 0)
            {
                Math.DivRem(dividend, --search_origin, out r);
            }

            return search_origin;
        }

        public static int FindNextDivisorWithoutReminder(int dividend, int search_origin)
        {
            var r = -1;

            while (r != 0)
            {
                Math.DivRem(dividend, ++search_origin, out r);
            }

            return search_origin;
        }
    }
}
