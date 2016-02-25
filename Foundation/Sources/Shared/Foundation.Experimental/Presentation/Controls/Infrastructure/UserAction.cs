using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace SquaredInfinity.Foundation.Presentation.Controls.Infrastructure
{
    public class UserAction : IUserAction
    {
        public FontFamily IconGlyphFontFamily { get; protected set; }
        public string IconGlyph { get; protected set; }
        public string DisplayName { get; protected set; }

        public Action<IUserActionParameters> ExecuteAction { get; protected set; }

        public UserAction()
        { }

        public UserAction(string displayName, string iconGlyph, Action<IUserActionParameters> action)
        {
            this.DisplayName = displayName;
            this.IconGlyph = iconGlyph;
            this.ExecuteAction = action;
        }

        public void Execute(IDictionary<string, object> parameters)
        {
            Execute(UserActionParameters.FromDictionary(parameters));
        }

        public void Execute(IUserActionParameters parameters)
        {
            ExecuteAction(parameters);
        }

    }
}
