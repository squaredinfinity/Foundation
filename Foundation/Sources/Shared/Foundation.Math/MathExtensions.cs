using System;
using System.Collections.Generic;
using System.Text;

namespace Foundation.Math
{
    public static class MathExtensions
    {
        public static int FindOddNumber(int search_origin, ForwardBackwardDirection direction)
        {
            var r = 0;

            Math.DivRem(search_origin, 2, out r);

            if (r == 1)
                return search_origin;

            if (direction == ForwardBackwardDirection.Forward)
                return search_origin + 1;
            else
                return search_origin - 1;

        }
    }
}
