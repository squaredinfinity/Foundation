using SquaredInfinity.ObjectExtensibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Presentation.ObjectExtensibility
{
    public class PresentationWrapper : 
        NotifyPropertyChangedObject, 
        IExtensibleObject<PresentationWrapper>
    {
        object _wrappedInstance;
        public object WrappedInstance
        {
            get { return _wrappedInstance; }
            set { TrySetThisPropertyValue(ref _wrappedInstance, value); }
        }

        public PresentationWrapper(object wrappedInstance)
        {
            _extensions = new ObservableExtensionCollection<PresentationWrapper>(this);
            this.WrappedInstance = wrappedInstance;
        }

        readonly IExtensionCollection<PresentationWrapper> _extensions;
        public IExtensionCollection<PresentationWrapper> Extensions
        {
            get { return _extensions; }
        }
    }
}
