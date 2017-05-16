using SquaredInfinity.ComponentModel;
using SquaredInfinity.Graphics.ColorSpaces;
using SquaredInfinity.Tagging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Presentation.Logic
{
    public class UserAction : NotifyPropertyChangedObject, IUserAction
    {
        string _iconGlyphFontFamily;
        public string IconGlyphFontFamily
        {
            get { return _iconGlyphFontFamily; }
            set { TrySetThisPropertyValue(ref _iconGlyphFontFamily, value); }
        }

        string _iconGlyph;
        public string IconGlyph
        {
            get { return _iconGlyph; }
            set { TrySetThisPropertyValue(ref _iconGlyph, value); }
        }

        string _displayName;
        public string DisplayName
        {
            get { return _displayName; }
            set { TrySetThisPropertyValue(ref _displayName, value); }
        }

        string _description;
        public string Description
        {
            get { return _description; }
            set { TrySetThisPropertyValue(ref _description, value); }
        }


        IColor _iconGlyphColor;
        public IColor IconGlyphColor
        {
            get { return _iconGlyphColor; }
            set { TrySetThisPropertyValue(ref _iconGlyphColor, value); }
        }


        LogicalOrder _order;
        public LogicalOrder Order
        {
            get { return _order; }
            set { TrySetThisPropertyValue(ref _order, value); }
        }

        readonly TagCollection _tags = new TagCollection();
        public ITagCollection Tags => _tags;


        public Action<UserActionParameters> ExecuteAction { get; set; }

        readonly List<IUserAction> _children = new List<IUserAction>();
        public List<IUserAction> Children {  get { return _children; } }
        IReadOnlyList<IUserAction> ICompositeUserAction.Children => Children;

        #region Constructors

        public UserAction(string displayName)
        {
            this.DisplayName = displayName;
        }

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
