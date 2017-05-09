using SquaredInfinity.Graphics.ColorSpaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Presentation.Logic
{
    public class UserAction : IUserAction
    {
        public string IconGlyphFontFamily { get; set; }

        public string IconGlyph { get; set; }

        public string DisplayName { get; set; }

        public IColor IconGlyphColor { get; set; }

        public LogicalOrder Order { get; set; }

        Action<UserActionParameters> ExecuteAction { get; set; }

        readonly List<IUserAction> _children = new List<IUserAction>();
        public List<IUserAction> Children {  get { return _children; } }
        IReadOnlyList<IUserAction> ICompositeUserAction.Children => Children;

        #region Constructors

        public UserAction()
        { }

        public UserAction(
            string displayName, 
            Action<UserActionParameters> action)
        {
            this.DisplayName = displayName;
            this.ExecuteAction = action;
        }

        public UserAction(
            string displayName, 
            string iconGlyphFontFamily, 
            string iconGlyph, 
            
            Action<UserActionParameters> action)
        {
            this.DisplayName = displayName;
            this.IconGlyphFontFamily = iconGlyphFontFamily;
            this.IconGlyph = iconGlyph;

            this.ExecuteAction = action;
        }

        #endregion

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
