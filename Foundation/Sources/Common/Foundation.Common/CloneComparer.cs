using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation
{
    public class CloneEqualityComparer<T>
        where T : ICloneEquatable<T>
    {
        static IEqualityComparer<T> _default = null;

        public static IEqualityComparer<T> Default
        {
            get
            {
                if(_default == null)
                {
                    _default = new DefaultCloneComparer<T>();
                }

                return _default;
            }
        }

        private class DefaultCloneComparer<T> : IEqualityComparer<T>
            where T : ICloneEquatable<T>
        {
            public bool Equals(T x, T y)
            {
                if (x == null || y == null)
                    return false;

                return x.IsCloneOf(y);
            }

            public int GetHashCode(T obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}
