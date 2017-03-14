using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Maths.Statistics
{
    public enum VarianceAlgorithm
    {
        /// <summary>
        /// Uses Online Algorithm for calculating Variance
        /// https://en.wikipedia.org/wiki/Algorithms_for_calculating_variance#Online_algorithm
        /// This algorithm should be able to handle large amount of samples, but some accuracy of the result may be lost due to rounding.
        /// </summary>
        Online,
        /// <summary>
        /// Keeps the sum of all samples and uses that sum to calculate Variance.
        /// Using this algorithm may cause double overflow with large amount of samples used.2
        /// https://en.wikipedia.org/wiki/Algorithms_for_calculating_variance#Na.C3.AFve_algorithm
        /// </summary>
        SumsAndSumSquares,
        /// <summary>
        /// Keeps the sum of all samples and uses that sum to calculate Variance.
        /// It uses invariant property of the variance to reduce chances of critical cancellation
        /// https://en.wikipedia.org/wiki/Algorithms_for_calculating_variance#Computing_shifted_data
        /// </summary>
        SumsAndSumSquaresWithShift
    }
}
