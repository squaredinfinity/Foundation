using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Diagnostics
{
    public interface IAttachedObject
    {
        string Name { get; }
        object Value { get; }
    }
}
