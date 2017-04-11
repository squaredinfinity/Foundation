using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Presentation.Logic
{
    public class UserAction : IUserAction
    {
        public string IconGlyphFontFamily { get; protected set; }
        public string IconGlyph { get; protected set; }

        public string DisplayName { get; protected set; }

        public LogicalOrder Order { get; protected set; }

        public Action<UserActionParameters> ExecuteAction { get; protected set; }

        public UserAction()
        { }

        public UserAction(string displayName, Action<UserActionParameters> action)
        {
            this.DisplayName = displayName;
            this.ExecuteAction = action;
        }

        public UserAction(string displayName, string iconGlyphFontFamily, string iconGlyph, Action<UserActionParameters> action)
        {
            this.DisplayName = displayName;
            this.IconGlyphFontFamily = iconGlyphFontFamily;
            this.IconGlyph = iconGlyph;
            this.ExecuteAction = action;
        }

        public void Execute(IDictionary<string, object> parameters)
        {
            Execute(new UserActionParameters(parameters));
        }

        public void Execute(UserActionParameters parameters)
        {
            ExecuteAction(parameters);
        }

    }
}
