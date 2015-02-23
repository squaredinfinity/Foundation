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
        public FeatureExportMetadataAttribute() { }

        public int ImportOrder { get; set; }

        public string BuildQuality { get; set; }
    }
}
