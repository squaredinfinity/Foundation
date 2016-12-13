using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// MAX: 0
    /// MIN: int.MaxValue - 1
    /// UNDEFINED: int.MaxValue
    /// </remarks>
    [DebuggerDisplay("{DebuggerDisplay}")]
    public struct LogicalOrder : IEquatable<LogicalOrder>, IComparable<LogicalOrder>
    {
        public const uint UNDEFINED = int.MaxValue;
        public static readonly LogicalOrder Undefined = UNDEFINED;
        public const uint LAST = UNDEFINED - 1;
        public static readonly LogicalOrder Last = UNDEFINED - 1;
        public static uint FIRST = 0;
        public static readonly LogicalOrder First = 0;

        public uint Value { get; set; }

        public LogicalOrder(uint value)
        {
            this.Value = value;
        }

        public LogicalOrder PushBackBy(uint delta)
        {
            // cast to long to avoid int overflow
            return (uint)Math.Min((long) Value + delta, Last);
        }

        public LogicalOrder BringForwardBy(uint delta)
        {
            // cast to long to avoid overflow
            return (uint)Math.Max((long)Value - delta, First);
        }

        #region Equality, Hash Code, Comparisons

        public int CompareTo(LogicalOrder other)
        {
            if (Value == UNDEFINED && other.Value == UNDEFINED)
                return 0;

            if (Value == UNDEFINED)
                return -1;

            if (other.Value == UNDEFINED)
                return 1;

            return Comparer<uint>.Default.Compare(Value, other.Value) * -1;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override bool Equals(object other)
        {
            if (!(other is LogicalOrder))
                return false;

            return Equals((LogicalOrder) other);
        }

        public bool Equals(LogicalOrder other)
        {
            return Value == other.Value;
        }

        public static bool operator ==(LogicalOrder a, LogicalOrder b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(LogicalOrder a, LogicalOrder b)
        {
            return !a.Equals(b);
        }

        public static bool operator >=(LogicalOrder a, LogicalOrder b)
        {
            return a.CompareTo(b) >= 0;
        }

        public static bool operator <=(LogicalOrder a, LogicalOrder b)
        {
            return a.CompareTo(b) <= 0;
        }

        public static bool operator >(LogicalOrder a, LogicalOrder b)
        {
            return a.CompareTo(b) > 0;
        }

        public static bool operator <(LogicalOrder a, LogicalOrder b)
        {
            return a.CompareTo(b) < 0;
        }

        #endregion

        #region Conversions

        public static implicit operator uint(LogicalOrder lo)
        {
            return lo.Value;
        }

        public static implicit operator LogicalOrder(uint val)
        {
            return new LogicalOrder(val);
        }

        #endregion

        public string DebuggerDisplay
        {
            get
            {
                if (object.Equals(this, LogicalOrder.First))
                    return $"FIRST ({Value})";

                if (object.Equals(this, LogicalOrder.Last))
                    return $"LAST ({Value})";

                if (object.Equals(this, LogicalOrder.Undefined))
                    return $"UNDEFINED ({Value})";

                return $"{Value}";
            }
        }
    }
}
