using SquaredInfinity.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.ObjectExtensibility
{
    public interface IExtensionCollection<TOwner> :
        ICollection<IExtension<TOwner>>
        where TOwner : IExtensibleObject<TOwner>
    {
        TExtension GetOrAdd<TExtension>(Func<TExtension> createValue) where TExtension : IExtension<TOwner>;
        TExtension GetByType<TExtension>();

        object this[Type extensionType] { get; }
        object this[string extensionTypeFullOrPartialName] { get; }
    }
}
