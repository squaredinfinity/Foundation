using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Maths.Statistics.Distributions
{
    public interface IDistribution
    {
        double GetNext();

        double PDF(double x);
        double CDF(double x);
    }
}
