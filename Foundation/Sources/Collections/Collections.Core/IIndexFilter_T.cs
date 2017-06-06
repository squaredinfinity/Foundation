using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Collections
{
    public interface IIndexFilter<TItem> : IReadOnlyList<TItem>, IIndexFilter
    { }
}
