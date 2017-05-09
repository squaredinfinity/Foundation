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

        void IUserAction.Execute(UserActionParameters parameters) => Action.Execute(parameters);
        void IUserAction.Execute(IDictionary<string, object> parameters) => Action.Execute(parameters);

        LogicalOrder IUserAction.Order => Action.Order;
        string IUserAction.DisplayName => Action.DisplayName;
        string IUserAction.IconGlyph => Action.IconGlyph;
        IColor IUserAction.IconGlyphColor => Action.IconGlyphColor;
        string IUserAction.IconGlyphFontFamily => Action.IconGlyphFontFamily;

        #endregion
    }
}
