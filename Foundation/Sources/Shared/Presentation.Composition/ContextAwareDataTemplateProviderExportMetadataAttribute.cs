using SquaredInfinity.Composition;
using SquaredInfinity.Presentation.DataTemplateSelectors;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Presentation.Composition
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    [MetadataAttribute]
    public class ContextAwareDataTemplateProviderExportMetadataAttribute : FeatureExportMetadataAttribute, IContextAwareDataTemplateProviderMetadata
    {
        public ContextAwareDataTemplateProviderExportMetadataAttribute(string contractName)
            : base(contractName, typeof(IContextAwareDataTemplateProvider))
        { }

        public ContextAwareDataTemplateProviderExportMetadataAttribute(string contractName, Type contractType)
            : base(contractName, contractType)
        { }

        public ContextAwareDataTemplateProviderExportMetadataAttribute(Type contractType)
            : base(contractType)
        { }

        public ContextAwareDataTemplateProviderExportMetadataAttribute()
            : base(typeof(IContextAwareDataTemplateProvider))
        { }
    }
}
