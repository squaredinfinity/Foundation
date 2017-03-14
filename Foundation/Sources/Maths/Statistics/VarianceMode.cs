using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Maths.Statistics
{
    public enum VarianceMode
    {
        /// <summary>
        /// \sigma^2 = {\sum(x - \bar x)^2 \over n}
        /// </summary>
        Biased = 0,
        /// <summary>
        /// Use Bessel's correction (use n-1 instead of n)
        /// \s^2 = {\sum(x - \bar x)^2 \over n - 1}
        /// https://en.wikipedia.org/wiki/Bessel%27s_correction
        /// </summary>
        Unbiased = 1
    }
}
