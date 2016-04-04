using System;
using SquaredInfinity.Foundation.Extensions;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.Foundation.Maths
{
    /// <summary>
    /// Represents an Interval of doubles
    /// https://en.wikipedia.org/wiki/Interval_(mathematics)
    /// </summary>
    public struct Interval
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
            get { return _flags.HasFlag(IntervalFlags.FromInclusive); }
            set
            {
                if (value == true)
                    _flags.Set(IntervalFlags.FromInclusive);
                else
                    _flags.Unset(IntervalFlags.FromInclusive);
            }
        }

        public bool IsToInclusive
        {
            get { return _flags.HasFlag(IntervalFlags.ToInclusive); }
            set
            {
                if (value == true)
                    _flags.Set(IntervalFlags.ToInclusive);
                else
                    _flags.Unset(IntervalFlags.ToInclusive);
            }
        }

        public bool IsEmpty
        {
            get
            {
                return 
                    _from == _to 
                    &&
                    // both ends exclusive
                    _flags == 0;
            }
        }

        static Interval()
        {
            Empty = new Interval();
        }

        public Interval(double inclusiveFrom, double inclusiveTo)
        {
            this._from = inclusiveFrom;            
            this._to = inclusiveTo;

            _flags = IntervalFlags.FromInclusive | IntervalFlags.ToInclusive;
        }

        public Interval(double from, bool isFromInclusive, double to, bool isToInclusive)
        {
            this._from = from;
            this._to = to;

            _flags = IntervalFlags.Exclusive;

            if (isFromInclusive)
                _flags.Set(IntervalFlags.FromInclusive);

            if (isToInclusive)
                _flags.Set(IntervalFlags.ToInclusive);
        }

        public bool Contains(double value)
        {
            if (double.IsNaN(value))
                return false;
            
                if(IsFromInclusive)
                {
                    if (!value.IsGreaterThanOrClose(From))
                        return false;
                }
                else
                {
                    if (!value.IsGreaterThan(From))
                        return false;
                }

            
                if(IsToInclusive)
                {
                    if (!value.IsLessThanOrClose(To))
                        return false;
                }
                else
                {
                    if (!value.IsLessThan(To))
                        return false;
                }

            return true;
        }

        /// <summary>
        /// Expands current interval exactly to fit provided interval.
        /// </summary>
        /// <param name="other"></param>
        public void Union(Interval other)
        {
            if (other._from < _from)
                _from = other._from;

            if (other._to > _to)
                _to = other._to;
        }
    }
}
