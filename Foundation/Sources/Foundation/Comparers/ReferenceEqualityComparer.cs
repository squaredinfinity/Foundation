using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Comparers
{
    public class ReferenceEqualityComparer : IEqualityComparer<object>
    {
        static IEqualityComparer<object> _instance = new ReferenceEqualityComparer();
        public static IEqualityComparer<object> Instance
        {
            get { return _instance; }
        }

        bool IEqualityComparer<object>.Equals(object x, object y)
        {
            if (x == null)
                return false;

            if (y == null)
                return false;

            return object.ReferenceEquals(x, y);
        }

        int IEqualityComparer<object>.GetHashCode(object obj)
        {
            if (obj == null)
                return int.MinValue;

            return obj.GetHashCode();
        }
    }
}
