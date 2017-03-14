using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Comparers
{
    public class ExceptionEqualityComparer : IEqualityComparer<Exception>
    {
        static IEqualityComparer<Exception> _default = new ExceptionEqualityComparer();
        public static IEqualityComparer<Exception> Default
        {
            get { return _default; }
        }

        public bool Equals(Exception x, Exception y)
        {
            if (x == null || y == null)
                return false;

            if (x.GetType() != y.GetType())
                return false;

            if (x.Message != y.Message)
                return false;

            if (x.StackTrace != y.StackTrace)
                return false;

            return true;
        }

        public int GetHashCode(Exception obj)
        {
            if (obj == null)
                return -1;

            unchecked
            {
                int hash = 17;

                if (obj.Message != null)
                    hash = hash * 23 + obj.Message.GetHashCode();

                if (obj.StackTrace != null)
                    hash = hash * 23 + obj.StackTrace.GetHashCode();

                return hash;
            }
        }
    }
}
