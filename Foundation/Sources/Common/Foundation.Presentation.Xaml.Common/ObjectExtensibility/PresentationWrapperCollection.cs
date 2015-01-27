using SquaredInfinity.Foundation.Collections;
using SquaredInfinity.Foundation.ObjectExtensibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Presentation.ObjectExtensibility
{
    public class PresentationWrapperCollection :
            XamlObservableCollectionEx<PresentationWrapper>,
            IExtensibleObject<PresentationWrapperCollection>
    {
        public PresentationWrapperCollection()
        {
            _extensions = new ObservableExtensionCollection<PresentationWrapperCollection>(this);
        }

        IExtensionCollection<PresentationWrapperCollection> _extensions;
        public IExtensionCollection<PresentationWrapperCollection> Extensions
        {
            get { return _extensions; }
        }
    }
}
