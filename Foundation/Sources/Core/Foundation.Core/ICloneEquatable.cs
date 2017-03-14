using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity
{
    public interface ICloneEquatable
    {
        int GetHashCode();
        bool IsCloneOf(object other);
    }

    public interface ICloneEquatable<T> : ICloneEquatable
    {
        bool IsCloneOf(T other);
    }
}
