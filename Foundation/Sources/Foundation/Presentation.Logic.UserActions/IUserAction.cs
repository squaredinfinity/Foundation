using SquaredInfinity.Graphics.ColorSpaces;
using SquaredInfinity.Tagging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Presentation.Logic
{
    public interface IUserAction : ICompositeUserAction
    {
        /// <summary>
        /// Return the name of font family to use for Icon Glyph
        /// </summary>
        /// <returns></returns>
        string IconGlyphFontFamily { get; }
        /// <summary>
        /// Return the glyph to use for this action
        /// </summary>
        string IconGlyph { get; }

        IColor IconGlyphColor { get; }

        /// <summary>
        /// Return display name of this action
        /// </summary>
        /// <returns></returns>
        string DisplayName { get; }
        string Description { get; }

        LogicalOrder Order { get; }

        ITagCollection Tags { get; }

        void Execute(UserActionParameters parameters);
        void Execute(IDictionary<string, object> parameters);
    }
}
