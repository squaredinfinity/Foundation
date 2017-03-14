using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Comparers
{
    public class DynamicComparer<T> : IComparer<T>
    {
        readonly Func<T, T, int> CompareMethod;

        public DynamicComparer(Func<T, T, int> compareMethod)
        {
            if (compareMethod == null)
                throw new ArgumentNullException();

            this.CompareMethod = compareMethod;
        }

        public int Compare(T x, T y)
        {
            return CompareMethod(x, y);
        }
    }
}
