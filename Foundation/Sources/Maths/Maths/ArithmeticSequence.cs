using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Maths
{
    public struct ArithmeticSequence
    {
        int _first;
        public int First => _first;

        int _delta;
        public int Delta => _delta;

        public ArithmeticSequence(int first, int delta)
        {
            _first = first;
            _delta = delta;
        }

        /// <summary>
        /// Returns sum of first n elements of the sequence
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public long Sum(int n) => (long)((n / 2.0) * (_first * 2.0 + (n - 1) * _delta));

        public long Sum(int i, int n) => (long)((n / 2.0) * (At(i) * 2.0 + (n - 1) * _delta));

        public int At(int n) => (int)(_first + (n - 1) * _delta);
    }
}
