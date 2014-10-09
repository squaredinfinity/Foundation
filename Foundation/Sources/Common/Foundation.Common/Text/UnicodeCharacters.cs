using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Text
{
    public static class UnicodeCharacters
    {
        public static class Specials
        {
            /// <summary>
            /// http://www.unicode.org/charts/PDF/U0000.pdf
            /// http://unicode.org/charts/PDF/U0080.pdf
            /// </summary>
            public static class Control
            {
                /// <summary>
                /// http://en.wikipedia.org/wiki/Non-breaking_space
                /// </summary>
                public static readonly string NoBreakSpace = "\u00A0";
            }

            /// <summary>
            /// http://unicode.org/charts/PDF/U2000.pdf
            /// </summary>
            public static class LayoutControls
            {
                /// <summary>
                /// http://en.wikipedia.org/wiki/Zero-width_space
                /// </summary>
                public static readonly string ZeroWidthSpace = "\u200B";
            }
        }
    }
}
