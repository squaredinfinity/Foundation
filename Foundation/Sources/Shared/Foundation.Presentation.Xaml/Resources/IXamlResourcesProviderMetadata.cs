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
        /// <summary>
        /// Default: int.MaxValue => will be loaded last, after any other resources with custom Import Order
        /// </summary>
        [DefaultValue(int.MaxValue)]
        int ImportOrder { get; }
    }
}
