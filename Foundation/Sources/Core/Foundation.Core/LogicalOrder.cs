using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// </remarks>
    public struct LogicalOrder : IEquatable<LogicalOrder>, IComparable<LogicalOrder>
    {
        static readonly uint UndefinedValue = int.MaxValue;

        public static readonly LogicalOrder Min = UndefinedValue + 1;
        public static readonly LogicalOrder Max = 0;
        public static readonly LogicalOrder Undefined = UndefinedValue;

        public uint Value { get; set; }

        public LogicalOrder(uint value)
        {
            this.Value = value;
        }

        public void DecreaseBy(uint delta)
        {
            // cast to long to avoid int overflow
            Value = (uint)Math.Min((long) Value + delta, Min);
        }

        public void IncreaseBy(uint delta)
        {
            // cast to long to avoid overflow
            Value = (uint)Math.Max((long)Value - delta, Max);
        }

        #region Equality, Hash Code, Comparisons

        public int CompareTo(LogicalOrder other)
        {
            if (Value == UndefinedValue && other.Value == UndefinedValue)
                return 0;

            if (Value == UndefinedValue)
                return -1;

            if (other.Value == UndefinedValue)
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
    }
}
