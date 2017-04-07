using System;
using System.Collections.Generic;
using SquaredInfinity.Extensions;
using System.Text;

namespace SquaredInfinity.Maths
{
    public struct IntervalInt32
    {
        public static readonly IntervalInt32 Empty;

        IntervalFlags _flags;
        public IntervalFlags Flags { get { return _flags; } set { _flags = value; } }

        Int32 _from;
        public Int32 From { get { return _from; } set { _from = value; } }

        Int32 _to;
        public Int32 To { get { return _to; } set { _to = value; } }

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
                    _from == _to
                    &&
                    // both ends exclusive
                    _flags == 0;
            }
        }

        static IntervalInt32()
        {
            Empty = new IntervalInt32();
        }

        public IntervalInt32(int inclusiveFrom, int inclusiveTo)
        {
            this._from = inclusiveFrom;            
            this._to = inclusiveTo;

            _flags = IntervalFlags.LeftClosed | IntervalFlags.RightClosed;
        }

        public IntervalInt32(int from, bool isFromInclusive, int to, bool itToInclusive)
        {
            this._from = from;
            this._to = to;

            _flags = 0;

            if (isFromInclusive)
                _flags.Set(IntervalFlags.LeftClosed);

            if (IsToInclusive)
                _flags.Set(IntervalFlags.RightClosed);
        }

        public bool Contains(int value)
        {
            if (_flags.HasFlag(IntervalFlags.LeftClosed))
            {
                if (!(value >= _from))
                    return false;
            }
            else
            {
                if (!(value > _from))
                    return false;
            }


            if (_flags.HasFlag(IntervalFlags.RightClosed))
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

        ///// <summary>
        ///// Returns a sum of all integers between From and To
        ///// </summary>
        ///// <returns></returns>
        //public ulong Sum()
        //{

        //}
    }
}
