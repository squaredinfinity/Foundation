using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace SquaredInfinity.Presentation.Controls
{
    public interface IUserAction
    {
        FontFamily IconGlyphFontFamily { get; }
        string IconGlyph { get; }
        string DisplayName { get; }

        Action<IUserActionParameters> ExecuteAction { get; }

        void Execute(IUserActionParameters parameters);
        void Execute(IDictionary<string, object> parameters);
    }
}
