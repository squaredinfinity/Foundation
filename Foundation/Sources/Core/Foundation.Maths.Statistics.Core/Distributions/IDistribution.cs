using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Maths.Statistics.Distributions
{
    public interface IDistribution
    {
        double GetNext();

        double PDF(double x);
    }
}
