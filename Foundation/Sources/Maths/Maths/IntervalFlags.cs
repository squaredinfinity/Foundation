using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Maths
{ 
    [Flags]
    public enum IntervalFlags : byte
    {
        /// <summary>
        /// Both ends exclusive (from, to) = { x | a < x < b }
        /// </summary>
        Open = 0,

        LeftClosed = 1,

        RightClosed = 2,

        /// <summary>
        /// Both ends inclusive [from, to] = { x | a <= x <= b }
        /// </summary>
        Closed = LeftClosed | RightClosed
    }
}
