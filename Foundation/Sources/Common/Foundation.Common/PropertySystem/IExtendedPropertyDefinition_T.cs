using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.PropertySystem
{
    public interface IExtendedPropertyDefinition<T> : IExtendedPropertyDefinition
    {
        Func<T> GetDefaultValue { get; }

        T GetActualValue(IExtendedPropertyContainer container);
    }
}
