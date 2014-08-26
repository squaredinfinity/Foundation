using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation
{
    public interface IIdentityEquatable
    {
        int GetIdentityHashCode();
        bool IdentityEquals(object other);
    }

    public interface IIdentityEquatable<T> : IIdentityEquatable
    {
        bool IdentityEquals(T other);
    }
}
