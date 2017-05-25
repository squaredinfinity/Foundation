using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Comparers
{
    class _Comparer : IComparer
    {
        public static readonly IComparer Default = new _Comparer();

        public int Compare(object a, object b)
        {
            if (a == b) return 0;
            if (a == null) return -1;
            if (b == null) return 1;

            IComparable ia = a as IComparable;
            if (ia != null)
                return ia.CompareTo(b);

            IComparable ib = b as IComparable;
            if (ib != null)
                return -ib.CompareTo(a);

            throw new ArgumentException("at least one argument should implement IComparable interface");
        }
    }
}
