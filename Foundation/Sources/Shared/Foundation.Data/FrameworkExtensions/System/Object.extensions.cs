using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Extensions
{
    public static class ObjectDbExtensions
    {
        public static T DbToClr<T>(this object dbValue)
        {
            if (object.Equals(dbValue, DBNull.Value))
                return default(T);

            return (T) dbValue;
        }
    }
}
