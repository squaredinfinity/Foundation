using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.PropertySystem
{
    public interface IExtendedPropertyDefinition
    {
        ExtendedPropertyUniqueIdentifier Id { get; }

        void SetValue(IExtendedPropertyContainer container, object value);

        void UnsetValue(IExtendedPropertyContainer container);

        void SetValue(IExtendedPropertyContainer container);

        bool GetIsValueSet(IExtendedPropertyContainer container);

        IExtendedProperty EnsurePropertyRegisteredWithContainer(IExtendedPropertyContainer container);
    }
}
