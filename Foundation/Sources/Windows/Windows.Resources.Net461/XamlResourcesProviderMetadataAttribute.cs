using SquaredInfinity;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Windows.Resources
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
    public class XamlResourcesProviderMetadataAttribute : ExportAttribute, IXamlResourcesProviderMetadata
    {
        public XamlResourcesProviderMetadataAttribute() :
            base(typeof(IXamlResourcesProvider))
        {
            ImportOrder = (uint)LogicalOrder.UNDEFINED;
        }

        public uint ImportOrder { get; set; }
    }
}
