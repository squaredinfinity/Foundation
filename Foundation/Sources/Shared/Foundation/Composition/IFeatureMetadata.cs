using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Composition
{
    public interface IFeatureMetadata
    {
        // Summary:
        //     Default: int.MaxValue => will be loaded last, after any other resources with
        //     custom Import Order
        [DefaultValue(2147483647)]
        int ImportOrder { get; }

        string BuildQuality { get; }
    }
}
