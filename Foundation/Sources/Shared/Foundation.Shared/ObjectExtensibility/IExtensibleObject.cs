using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.ObjectExtensibility
{
    public interface IExtensibleObject<TExtensibleObject>
        where TExtensibleObject : IExtensibleObject<TExtensibleObject>
    {
        IExtensionCollection<TExtensibleObject> Extensions { get; }
    }
}
