using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Foundation.Types.Mapping
{
    public interface ITypeMapper
    {
        T DeepClone<T>(T source) where T : class, new();
        object DeepClone(object source);
    }
}
