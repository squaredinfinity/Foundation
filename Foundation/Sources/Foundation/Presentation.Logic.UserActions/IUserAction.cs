using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Presentation.Logic
{
    public interface IUserAction
    {
        string IconGlyphFontFamily { get; }
        string IconGlyph { get; }

        string DisplayName { get; }

        LogicalOrder Order { get; }

        Action<UserActionParameters> ExecuteAction { get; }

        void Execute(UserActionParameters parameters);
        void Execute(IDictionary<string, object> parameters);
    }
}
