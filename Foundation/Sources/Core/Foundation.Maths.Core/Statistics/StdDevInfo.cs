using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Maths.Statistics
{
    public struct StdDevInfo
    {
        VarianceInfo _variance;
        public VarianceInfo Variance {  get { return _variance; } set { _variance = value; } }

        public double StdDev
        {
            get
            {
                return Math.Sqrt(Variance);
            }
        }

        public static implicit operator double (StdDevInfo sd)
        {
            return sd.StdDev;
        }

        public override string ToString()
        {
            return StdDev.ToString();
        }

        public string ToString(string format)
        {
            return StdDev.ToString(format);
        }
    }
}                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                    //       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