using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Foundation
{
    public class CompositeComparer<T> : IComparer<T>
    {
        public readonly List<Comparison<T>> Comparisons = new List<Comparison<T>>();
        
        public void AddDefaultNullCheckComparison()
        {
            Comparisons.Add((x, y) =>
            {
                if (x == null)
                {
                    if (y == null)
                    {
                        // If x is null and y is null, they're equal.  
                        return 0;
                    }
                    else
                    {
                        // If x is null and y is not null, y is greater.  
                        return -1;
                    }
                }
                else
                {
                    // If x is not null and y is null, x is greater.
                    if (y == null)
                    {
                        return 1;
                    }
                    else
                    {
                        return 0; // they are equal as far as this comparer is concerned
                    }
                }
            });
        }

        public int Compare(T x, T y)
        {
            if (Comparisons.Count == 0)
                return 0;

            int result = 0;

            foreach (var comparison in Comparisons)
            {
                var r = comparison(x, y);

                if (r == 0)
                    continue;
                else
                {
                    result = r;
                    break;
                }
            }

            return result;
        }
    }
}
