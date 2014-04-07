using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Foundation.Extensions
{
    public static class IEnumerableExtensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> list)
        {
            if (list == null)
                return true;

            return !list.Any();
        }

        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> list)
        {
            if (list == null)
                return new EmptyEnumerator<T>();

            return list;
        }

        class EmptyEnumerator<T> : IEnumerator<T>, IEnumerable<T>, IEnumerator, IEnumerable
        {
            public object Current
            {
                get { return null; }
            }

            public bool MoveNext()
            {
                return false;
            }

            public void Reset()
            {
            }

            public IEnumerator GetEnumerator()
            {
                return this;
            }

            T IEnumerator<T>.Current
            {
                get { return default(T); }
            }

            public void Dispose()
            {
            }

            IEnumerator<T> IEnumerable<T>.GetEnumerator()
            {
                return this;
            }
        }
    }
}
