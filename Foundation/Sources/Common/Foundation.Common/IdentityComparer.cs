using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation
{
    public class IdentityComparer<T>
        where T : IIdentityEquatable<T>
    {
        static IEqualityComparer<T> _default = null;

        public static IEqualityComparer<T> Default
        {
            get
            {
                if(_default == null)
                {
                    _default = new DefaultIdentityComparer<T>();
                }

                return _default;
            }
        }

        private class DefaultIdentityComparer<T> : IEqualityComparer<T>
            where T : IIdentityEquatable<T>
        {
            public bool Equals(T x, T y)
            {
                if (x == null || y == null)
                    return false;

                return x.IdentityEquals(y);
            }

            public int GetHashCode(T obj)
            {
                return obj.GetIdentityHashCode();
            }
        }
    }
}
