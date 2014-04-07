using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Extensions
{
    public static class ICollectionExtensions
    {
        public static void AddIfNotNull<T>(this ICollection<T> collection, T value)
        {
            if (value == null)
                return;

            collection.Add(value);
        }
    }
}
