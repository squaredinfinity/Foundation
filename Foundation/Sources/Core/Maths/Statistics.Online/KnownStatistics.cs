using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Maths.Statistics
{
    public enum KnownStatistics
    {
        None = 0x0,
        Count = 0x1,
        Min = 0x2,
        Max = 0x4,
        Range = 0x8,
        Mean = 0x10,
        Variance = 0x20,
        StdDev = 0x40,
        LastValue = 0x80,

        All = Count | Min | Max | Range | Mean | Variance | StdDev | LastValue
    }
}
