using SquaredInfinity.Graphics.ColorSpaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Presentation.Logic
{
    public class UserActionState
    {
        public string UniqueName { get; private set; }
        public IUserAction Action { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string IconGlyph { get; set; }
        public IColor IconGlyphColor { get; set; }
        public string IconGlyphFontFamily { get; set; }

        public UserActionState(string uniqueName)
        {
            this.UniqueName = uniqueName;
        }
    }
}
