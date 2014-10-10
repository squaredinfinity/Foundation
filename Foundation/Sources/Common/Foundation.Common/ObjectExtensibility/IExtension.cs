using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.ObjectExtensibility
{
    public interface IExtension<TOwner>
            where TOwner : IExtensibleObject<TOwner>
    {
        void Attach(TOwner owner);
        void Detach(TOwner owner);
    }
}
