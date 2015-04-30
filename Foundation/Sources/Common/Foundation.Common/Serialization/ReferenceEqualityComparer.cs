using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Serialization
{
    public class ReferenceEqualityComparer : IEqualityComparer<object>
    {
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
