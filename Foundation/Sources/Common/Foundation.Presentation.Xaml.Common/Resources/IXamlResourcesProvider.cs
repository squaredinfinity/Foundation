using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Presentation.Resources
{
    public interface IXamlResourcesProvider
    {
        void LoadAndMergeResources();
    }
}
