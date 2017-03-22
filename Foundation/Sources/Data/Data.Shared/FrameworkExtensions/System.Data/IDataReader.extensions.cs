using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Extensions
{
    public static class IDataReaderExtensions
    {
        public static IReadOnlyList<TItem> ReadAll<TItem>(this IDataReader reader, Func<IDataReader, TItem> convert)
        {
            List<TItem> result = new List<TItem>();

            while(reader.Read())
            {
                result.Add(convert(reader));
            }

            return result;
        }
    }
}
