using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Presentation.Xaml
{
    public static class All
    {
        public static void LoadAndMergeResourcesFromThisAssembly()
        {
            Resources.LoadAndMergeCompiledResourceDictionaryFromThisAssembly(@"Xaml\All.xaml");
        }
    }
}
