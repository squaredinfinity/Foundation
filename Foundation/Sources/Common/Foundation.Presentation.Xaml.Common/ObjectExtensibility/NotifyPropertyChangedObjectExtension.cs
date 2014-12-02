using SquaredInfinity.Foundation.ObjectExtensibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Presentation.ObjectExtensibility
{
    public abstract class NotifyPropertyChangedObjectExtension<TOwner> :
        NotifyPropertyChangedObject,
        IExtension<TOwner>
        where TOwner : IExtensibleObject<TOwner>
    {
        public TOwner Owner { get; private set; }

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
