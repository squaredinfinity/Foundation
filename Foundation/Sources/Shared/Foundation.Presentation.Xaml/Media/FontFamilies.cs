using SquaredInfinity.Foundation.Presentation.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SquaredInfinity.Foundation.Presentation.Media
{
    public static class FontFamilies
    {
        public static FontFamily FontsiFontFamily { get; private set; }

        static FontFamilies()
        {
            FontsiFontFamily = new FontFamily(ResourcesManager.GetAbsoluteResourceUriFromThisAssembly(@""), "./fonts/#fontsi");
        }

    }
}
