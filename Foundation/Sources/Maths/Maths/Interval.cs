using System;
using SquaredInfinity.Foundation.Extensions;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace SquaredInfinity.Foundation.Maths
{
    /// <summary>
    /// Represents an Interval of doubles
    /// https://en.wikipedia.org/wiki/Interval_(mathematics)
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay}")]
    public struct Interval : IEquatable<Interval>
    {
        public static readonly Interval Empty;

        IntervalFlags _flags;
        public IntervalFlags Flags {  get { return _flags; } set { _flags = value; } }

        public double Span
        {
            get
            {
                if (_from.IsInfinityOrNaN() || _to.IsInfinityOrNaN())
                    return double.NaN;

                return _to - _from;
            }
        }

        double _from;
        public double From { get { return _from; } set { _from = value; } }
        double _to;
        public double To { get { return _to; } set { _to = value; } }

        public bool IsFromInclusive
        {
            get { return _flags.HasFlag(IntervalFlags.LeftClosed); }
            set
            {
                if (value == true)
                    _flags.Set(IntervalFlags.LeftClosed);
                else
                    _flags.Unset(IntervalFlags.LeftClosed);
            }
        }

        public bool IsToInclusive
        {
            get { return _flags.HasFlag(IntervalFlags.RightClosed); }
            set
            {
                if (value == true)
                    _flags.Set(IntervalFlags.RightClosed);
                else
                    _flags.Unset(IntervalFlags.RightClosed);
            }
        }

        public bool IsEmpty
        {
            get
            {
                return
                    // NaN, NaN - both ends
                    (double.IsNaN(_from) && double.IsNaN(_to))
                    ||
                    // (from, from) - exclisve both ends
                    (_from == _to && _flags == IntervalFlags.Open)
                    ||
                    // [from, from) or (from, from] - one end exclusive
                    (_from == _to && (_flags == IntervalFlags.LeftClosed || _flags == IntervalFlags.RightClosed))
                    ||
                    // [to, from] or (to, from), where to > from
                    (_from > _to);
            }
        }

        // [from, from] - inclusive both ends
        public bool IsDegenerate => _from == _to && _flags == IntervalFlags.Closed;
        public bool IsProper => !IsEmpty && !IsDegenerate;
        public bool IsOpen => _flags == IntervalFlags.Open;
        public bool IsClosed => _flags == IntervalFlags.Closed;

        static Interval()
        {
            Empty = new Interval(double.NaN, double.NaN);
        }

        public Interval(double inclusiveFrom, double inclusiveTo)
        {
            this._from = inclusiveFrom;            
            this._to = inclusiveTo;

            _flags = IntervalFlags.Closed;
        }

        public Interval(double from, bool isFromInclusive, double to, bool isToInclusive)
        {
            this._from = from;
            this._to = to;

            _flags = IntervalFlags.Open;
            
            if (isFromInclusive)
                _flags = _flags.Set(IntervalFlags.LeftClosed);

            if (isToInclusive)
                _flags = _flags.Set(IntervalFlags.RightClosed);
        }

        #region Contains 

        public bool Contains(double value)
        {
            if (double.IsNaN(value))
                return false;
            
                if(IsFromInclusive)
                {
                    if (!value.IsGreaterThanOrClose(_from))
                        return false;
                }
                else
                {
                    if (!value.IsGreaterThan(_from))
                        return false;
                }

            
                if(IsToInclusive)
                {
                    if (!value.IsLessThanOrClose(_to))
                        return false;
                }
                else
                {
                    if (!value.IsLessThan(_to))
                        return false;
                }

            return true;
        }

        #endregion

        #region Expand

        /// <summary>
        /// Creates new Interval by expanding this interval exactly to fit provided interval.
        /// </summary>
        /// <param name="other"></param>
        public Interval Expand(Interval other)
        {
            var new_from = _from;
            var new_to = _to;

            if (other._from < _from || double.IsNaN(_from))
                new_from = other._from;

            if (other._to > _to || double.IsNaN(_to))
                new_to = other._to;

            return new Interval(new_from, IsFromInclusive, new_to, IsToInclusive);
        }

        #endregion

        #region Union

        /// <summary>
        /// The union of two intervals is an interval if and only if they have a non - empty intersection or an open end - point of one interval is a closed end-point of the other
        /// (e.g. (a, b) \cup[b, c] = (a, c]).
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        //public static Interval Union(Interval a, Interval b)
        //{

        //}

        #endregion

        #region Intersection

        public Interval Intersect(Interval b)
        {
            return Intersect(this, b);
        }

        public static Interval Intersect(Interval a, Interval b)
        {
            if (a.IsEmpty || b.IsEmpty)
                return Interval.Empty;

            var a_from = new IntervalEdge(a._from, a.IsFromInclusive);
            var b_from = new IntervalEdge(b._from, b.IsFromInclusive);

            var a_to = new IntervalEdge(a._to, a.IsToInclusive);
            var b_to = new IntervalEdge(b._to, b.IsToInclusive);

            var from = MathEx.Max(a_from, b_from);
            var to = MathEx.Min(a_to, b_to);

            return new Interval(from.Value, from.IsClosed, to.Value, to.IsClosed);
        }

        public bool IntersectsWith(Interval other)
        {
            if (this.IsEmpty || other.IsEmpty)
                return false;

            if(other.IsFromInclusive)
            {
                if (Contains(other._from))
                    return true;
            }
            else
            {
                if (other._from.IsGreaterThan(_from) && other._from.IsLessThan(_to))
                    return true;
            }

            if (other.IsToInclusive)
            {
                if (Contains(other._to))
                    return true;
            }
            else
            {
                if (other._to.IsGreaterThan(_from) && other._to.IsLessThan(_to))
                    return true;
            }

            return false;
        }

        #endregion

        #region LinSpace

        /// <summary>
        /// Returns an array of 100 evenly spaced points between From and To of this interval.
        /// If interval is Closed, From and To will be included in returned points.
        /// </summary>
        /// <returns></returns>
        public double[] LinSpace()
        {
            return LinSpace(n: 100);
        }

        /// <summary>
        /// Returns an array of n evenly spaced points between From and To of this interval.
        /// If interval is Closed, From and To will be included in returned points.
        /// From edge of the interval must be a valid number (i.e. not Infinity or NaN)
        /// </summary>
        /// <returns></returns>
        public double[] LinSpace(int n)
        {
            if (_from.IsInfinityOrNaN())
                throw new ArgumentOutOfRangeException($"Left side of the interval cannot be Infinity or NaN");

            var result = new double[n];

            // if interval is open, increase number of points between ranges, since edges will not be included
            if (!_flags.HasFlag(IntervalFlags.LeftClosed))
                n++;
            if (!_flags.HasFlag(IntervalFlags.RightClosed))
                n++;

            var spacing = (_to - _from) / (n - 1);

            var current_value = _from;

            if (!Flags.HasFlag(IntervalFlags.LeftClosed))
                current_value += spacing;

            for(int i = 0; i < result.Length; i++)
            {
                result[i] = current_value;
                current_value += spacing;
            }

            return result;
        }

        #endregion

        #region Equality + HashCode

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;

                hash = hash * 23 + Flags.GetHashCode();
                hash = hash * 23 + From.GetHashCode();
                hash = hash * 23 + To.GetHashCode();

                return hash;
            }
        }

        public override bool Equals(object other)
        {
            if (!(other is Interval))
                return false;

            return this.Equals((Interval) other);
        }

        public bool Equals(Interval other)
        {
            return
                Enum.Equals(Flags, other.Flags)
                &&
                double.Equals(From, other.From)
                &&
                double.Equals(To, other.To);
        }

        public static bool operator ==(Interval a, Interval b)
        {
            return Interval.Equals(a, b);
        }

        public static bool operator !=(Interval a, Interval b)
        {
            return !Interval.Equals(a, b);
        }

        #endregion

        #region To String + Debugger Display

        public string DebuggerDisplay
        {
            get { return ToString(); }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append(IsFromInclusive ? "[" : "(");
            sb.Append(_from);
            sb.Append(",");
            sb.Append(_to);
            sb.Append(IsToInclusive ? "]" : ")");

            return sb.ToString();
        }

        #endregion
    }
}
