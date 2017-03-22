using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Comparers
{
    public interface ICloneEquatable<T> : ICloneEquatable
    {
        bool IsCloneOf(T other);
    }
}
