using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SquaredInfinity.Windows.Resources
{
    public interface IXamlResourcesProvider
    {
        IEnumerable<ResourceDictionary> LoadResources();
    }
}
