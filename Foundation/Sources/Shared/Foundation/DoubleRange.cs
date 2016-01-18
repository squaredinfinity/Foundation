using System;
using SquaredInfinity.Foundation.Extensions;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.Foundation
{
    public class DoubleRange
    {
        double? _from;
        public double? From
        {
            get { return _from; }
            set { _from = value; }
        }

        double? _to;
        public double? To
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

        public DoubleRange(double? inclusiveFrom, double? inclusiveTo)
        {
            this.From = inclusiveFrom;
            this.IsFromInclusive = true;

            this.To = inclusiveTo;
            this.IsToInclusive = true;
        }

        public bool IsWithinRange(double value)
        {
            if (value.IsInfinityOrNaN())
                return false;

            if(From != null)
            {
                if(IsFromInclusive)
                {
                    if (!value.IsGreaterThanOrClose(From.Value))
                        return false;
                }
                else
                {
                    if (!value.IsGreaterThan(From.Value))
                        return false;
                }
            }

            if(To != null)
            {
                if(IsToInclusive)
                {
                    if (!value.IsLessThanOrClose(To.Value))
                        return false;
                }
                else
                {
                    if (!value.IsLessThan(To.Value))
                        return false;
                }
            }

            return true;
        }
    }
}
