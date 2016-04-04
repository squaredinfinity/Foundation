using System;
using System.Collections.Generic;
using SquaredInfinity.Foundation.Extensions;
using System.Text;

namespace SquaredInfinity.Foundation.Maths
{
    public struct Int32Interval
    {
        public static readonly Int32Interval Empty;

        IntervalFlags _flags;
        public IntervalFlags Flags { get { return _flags; } set { _flags = value; } }

        Int32 _from;
        public Int32 From { get { return _from; } set { _from = value; } }

        Int32 _to;
        public Int32 To { get { return _to; } set { _to = value; } }

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

        static Int32Interval()
        {
            Empty = new Int32Interval();
        }

        public Int32Interval(int inclusiveFrom, int inclusiveTo)
        {
            this._from = inclusiveFrom;            
            this._to = inclusiveTo;

            _flags = IntervalFlags.FromInclusive | IntervalFlags.ToInclusive;
        }

        public Int32Interval(int from, bool isFromInclusive, int to, bool itToInclusive)
        {
            this._from = from;
            this._to = to;

            _flags = 0;

            if (isFromInclusive)
                _flags.Set(IntervalFlags.FromInclusive);

            if (IsToInclusive)
                _flags.Set(IntervalFlags.ToInclusive);
        }

        public bool Contains(int value)
        {
            if (_flags.HasFlag(IntervalFlags.FromInclusive))
            {
                if (!(value >= _from))
                    return false;
            }
            else
            {
                if (!(value > _from))
                    return false;
            }


            if (_flags.HasFlag(IntervalFlags.ToInclusive))
            {
                if (!(value <= _to))
                    return false;
            }
            else
            {
                if (!(value < _to))
                    return false;
            }

            return true;
        }
    }
}
