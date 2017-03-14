using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Diagnostics
{
    public interface IAttachedObject
    {
        string Name { get; }
        object Value { get; }
    }
}
