using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Maths.Statistics
{
    /// <summary>
    /// Contains details about the Range.
    /// This includes Range itself as well as Min and Max values.
    /// </summary>
    public struct RangeInfo
    {
        Interval _interval;
        public Interval Interval { get { return _interval; } }

        public double Min { get { return _interval.From; } set { _interval.From = value; } }
        public double Max { get { return _interval.To; } set { _interval.To = value; } }
        public double Range { get { return _interval.Span; } }

        public RangeInfo(double min, double max)
        {
            _interval = new Interval(min, max);
        }

        public static implicit operator double(RangeInfo ri)
        {
            return ri.Range;
        }

        public override string ToString()
        {
            return Interval.ToString();
        }
    }
}
