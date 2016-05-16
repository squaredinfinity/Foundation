using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Presentation.Resources
{
    public interface IXamlResourcesProviderMetadata
    {
        uint ImportOrder { get; }
    }
}
