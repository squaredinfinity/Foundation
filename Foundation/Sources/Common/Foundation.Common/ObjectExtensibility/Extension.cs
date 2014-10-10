using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.ObjectExtensibility
{
    public abstract class Extension<TOwner> :
            IExtension<TOwner>
            where TOwner : IExtensibleObject<TOwner>
    {
        TOwner Owner { get; set; }

        public void Attach(TOwner owner)
        {
            this.Owner = owner;

            OnAttached(owner);
        }

        protected virtual void OnAttached(TOwner owner)
        { }

        public void Detach(TOwner owner)
        {
            Owner = default(TOwner);

            OnDetached(owner);
        }

        protected virtual void OnDetached(TOwner owner)
        { }
    }
}
