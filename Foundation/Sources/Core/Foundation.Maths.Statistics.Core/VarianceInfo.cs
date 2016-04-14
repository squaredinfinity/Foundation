using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Maths.Statistics
{
    public struct VarianceInfo
    {
        public static readonly VarianceInfo NaN;

        double _m1;
        public double M1 { get { return _m1; } set { _m1 = value; } }

        public double Mean { get { return _m1; } }

        double _m2;
        /// <summary>
        /// M2 - Sum of squares of differences {\sum(x - \bar x)^2} (i.e. just the nominator of a Variance)
        /// </summary>
        public double M2 { get { return _m2; }  set { _m2 = value; } }

        double _m2Denominator;
        public double M2Denominator {  get { return _m2Denominator; } set { _m2Denominator = value; } }

        Int64 _count;
        public Int64 Count { get { return _count; } set { _count = value; } }

        public double Variance
        {
            get
            {
                if (double.IsNaN(_m2) || double.IsNaN(_m2Denominator))
                    return double.NaN;

                return _m2 / _m2Denominator;
            }
        }

        public VarianceInfo(double m1, double m2, double m2Denominator)
        {
            this._count = 0;
            this._m1 = m1;
            this._m2 = m2;
            this._m2Denominator = m2Denominator;
        }

        static VarianceInfo()
        {
            NaN = new VarianceInfo(double.NaN, double.NaN, double.NaN);
        }

        public static implicit operator double(VarianceInfo vi)
        {
            return vi.Variance;
        }

        public override string ToString()
        {
            return Variance.ToString();
        }

        public string ToString(string format)
        {
            return Variance.ToString(format);
        }
    }
}
