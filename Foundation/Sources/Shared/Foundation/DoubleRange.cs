using System;
using SquaredInfinity.Foundation.Extensions;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.Foundation
{
    public class DoubleRange
    {
        public double Span
        {
            get
            {
                if (double.IsNaN(From) || double.IsNaN(To))
                    return double.NaN;

                if (From.IsInfinity() || To.IsInfinity())
                    return double.PositiveInfinity;

                return To - From;
            }
        }

        double _from;
        public double From
        {
            get { return _from; }
            set { _from = value; }
        }

        double _to;
        public double To
        {
            get { return _to; }
            set { _to = value; }
        }

        bool _isFromInclusive = true;
        public bool IsFromInclusive
        {
            get { return _isFromInclusive; }
            set { _isFromInclusive = value; }
        }

        bool _isToInclusive = true;
        public bool IsToInclusive
        {
            get { return _isToInclusive; }
            set { _isToInclusive = value; }
        }

        public DoubleRange()
        { }

        public DoubleRange(double inclusiveFrom, double inclusiveTo)
        {
            this.From = inclusiveFrom;
            this.IsFromInclusive = true;

            this.To = inclusiveTo;
            this.IsToInclusive = true;
        }

        public bool IsWithinRange(double value)
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
        /// Expands current range exactly to fit provided range.
        /// </summary>
        /// <param name="range"></param>
        public void Union(DoubleRange range)
        {
            if (range == null)
                return;

            if (range.From < From)
                From = range.From;

            if (range.To > To)
                To = range.To;
        }
    }
}
