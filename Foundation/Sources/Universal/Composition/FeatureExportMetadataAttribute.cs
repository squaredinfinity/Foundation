using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Composition
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    [MetadataAttribute]
    public class FeatureExportMetadataAttribute : ExportAttribute, IFeatureMetadata
    {
        public FeatureExportMetadataAttribute(string contractName)
            : base(contractName)
        { }

        public FeatureExportMetadataAttribute(string contractName, Type contractType)
            : base(contractName, contractType)
        { }

        public FeatureExportMetadataAttribute(Type contractType)
            : base(contractType)
        { }

        public FeatureExportMetadataAttribute()
        { }

        public uint ImportOrder { get; set; }

        public string BuildQuality { get; set; }
    }
}
