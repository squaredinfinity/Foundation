using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Diagnostics.Formatters
{
    public interface IFormatterCollection : ICollection<IFormatter>
    {
        bool TryFindByName(string name, out IFormatter result);
    }
}
