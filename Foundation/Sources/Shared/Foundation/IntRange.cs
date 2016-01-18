using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.Foundation
{
    public class IntRange
    {
        int? _from;
        public int? From
        {
            get { return _from; }
            set { _from = value; }
        }

        int? _to;
        public int? To
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

        public IntRange()
        { }

        public IntRange(int? inclusiveFrom, int? inclusiveTo)
        {
            this.From = inclusiveFrom;
            this.IsFromInclusive = true;

            this.To = inclusiveTo;
            this.IsToInclusive = true;
        }

        public bool IsWithinRange(int value)
        {
            if (From != null)
            {
                if (IsFromInclusive)
                {
                    if (!(value >= From.Value))
                        return false;
                }
                else
                {
                    if (!(value > From.Value))
                        return false;
                }
            }

            if (To != null)
            {
                if (IsToInclusive)
                {
                    if (!(value <= To.Value))
                        return false;
                }
                else
                {
                    if (!(value < To.Value))
                        return false;
                }
            }

            return true;
        }
    }
}
