using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Maths
{
    public struct ArithmeticSequence
    {
        double _first;
        public double First => _first;

        double _delta;
        public double Delta => _delta;

        public ArithmeticSequence(double first, double delta)
        {
            _first = first;
            _delta = delta;
        }

        /// <summary>
        /// Returns sum of first n elements of the sequence
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public double Sum(int n) => ((n / 2.0) * (_first * 2.0 + (n - 1) * _delta));


        /// <summary>
        /// 
        /// </summary>
        /// <param name="i">1-based index of a start element of the sequence</param>
        /// <param name="n">number of elements to sum</param>
        /// <returns></returns>
        public double Sum(int i, int n) => ((n / 2.0) * (At(i) * 2.0 + (n - 1) * _delta));

        public double At(int n) => (_first + (n - 1) * _delta);
    }
}
