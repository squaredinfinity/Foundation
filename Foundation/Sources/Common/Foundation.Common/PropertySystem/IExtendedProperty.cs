using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.PropertySystem
{
    public interface IExtendedProperty
    {
        IExtendedPropertyDefinition PropertyDefinition { get; }
        bool IsValueSet { get; set; }
        object Value { get; set; }
        object ActualValue { get; }

    }
}
