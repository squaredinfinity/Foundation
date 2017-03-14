using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Composition
{
    public interface IFeatureMetadata
    {
        // Summary:
        //     Default: LogicalOrder.UndefinedValue, will be loaded in low priority, but before LogicalValue.Min
        [DefaultValue(LogicalOrder.UNDEFINED)]
        uint ImportOrder { get; }

        string BuildQuality { get; }
    }
}
