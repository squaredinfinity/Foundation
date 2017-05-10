using SquaredInfinity.ComponentModel;
using SquaredInfinity.Graphics.ColorSpaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Presentation.Logic
{
    public class DynamicUserAction : NotifyPropertyChangedObject, IUserAction, IDynamicUserAction
    {
        IUserAction _action;
        public IUserAction Action
        {
            get { return _action; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(Action));

                _action = value;
                base.RaiseAllPropertiesChanged();
            }
        }

        #region Constructors

        public DynamicUserAction()
        { }

        public DynamicUserAction(IUserAction action)
        {
            Action = action;
        }

        #endregion

        #region ICompositeUserAction

        public IReadOnlyList<IUserAction> Children => Action.Children;

        #endregion

        #region IUserAction

        public void Execute(UserActionParameters parameters) => Action.Execute(parameters);
        public void Execute(IDictionary<string, object> parameters) => Action.Execute(parameters);

        public LogicalOrder Order => Action.Order;
        public string DisplayName => Action.DisplayName;
        public string Description => Action.Description;
        public string IconGlyph => Action.IconGlyph;
        public IColor IconGlyphColor => Action.IconGlyphColor;
        public string IconGlyphFontFamily => Action.IconGlyphFontFamily;

        #endregion
    }
}
