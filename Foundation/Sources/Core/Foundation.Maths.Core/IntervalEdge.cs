using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SquaredInfinity.Foundation.Extensions;
using System.Diagnostics;

namespace SquaredInfinity.Foundation.Maths
{
    [DebuggerDisplay("{DebuggerDisplay}")]
    struct IntervalEdge : IEquatable<IntervalEdge>, IComparable<IntervalEdge>
    {
        public double Value { get; private set; }
        public bool IsClosed { get; private set; }

        public IntervalEdge(double value, bool isClosed)
        {
            this.Value = value;
            this.IsClosed = isClosed;
        }

        #region Equality + Hash Code + Comparisons

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;

                hash = hash * 23 + Value.GetHashCode();
                hash = hash * 23 + IsClosed.GetHashCode();

                return hash;
            }
        }

        public override bool Equals(object other)
        {
            if (!(other is IntervalEdge))
                return false;

            return base.Equals((IntervalEdge)other);
        }

        public bool Equals(IntervalEdge other)
        {
            return
                double.Equals(other.Value, Value)
                &&
                bool.Equals(other.IsClosed, IsClosed);
        }

        public int CompareTo(IntervalEdge other)
        {
            if (Value.IsCloseTo(other.Value))
            {
                if (IsClosed == other.IsClosed)
                    return 0;

                if (IsClosed)
                    return -1;
                else
                    return 1;
            }
            else
            {
                return Value.CompareTo(other.Value);
            }
        }

        public static bool operator ==(IntervalEdge a, IntervalEdge b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(IntervalEdge a, IntervalEdge b)
        {
            return !a.Equals(b);
        }

        public static bool operator >=(IntervalEdge a, IntervalEdge b)
        {
            return a.CompareTo(b) >= 0;
        }

        public static bool operator <=(IntervalEdge a, IntervalEdge b)
        {
            return a.CompareTo(b) <= 0;
        }

        public static bool operator >(IntervalEdge a, IntervalEdge b)
        {
            return a.CompareTo(b) < 0;
        }

        public static bool operator <(IntervalEdge a, IntervalEdge b)
        {
            return a.CompareTo(b) > 0;
        }

        #endregion

        #region To String + Debugger Display

        public override string ToString()
        {
            return $"{Value}, {(IsClosed ? "closed" : "open")}";
        }

        public string DebuggerDisplay => ToString();

        #endregion
    }
}
