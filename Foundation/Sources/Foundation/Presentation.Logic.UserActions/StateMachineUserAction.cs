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
    public class StateMachineUserAction : NotifyPropertyChangedObject, IStateMachineUserAction, IDynamicUserAction
    {
        #region IDynamicUserAction

        public IUserAction Action
        {
            get { return CurrentState.Action; }
            set { throw new NotSupportedException("Use .SetState() to change current action based on current state"); }
        }

        #endregion 

        readonly TagCollection _properties = new TagCollection();
        public TagCollection Properties => _properties;

        readonly Dictionary<string, UserActionState> States = new Dictionary<string, UserActionState>();

        UserActionState _currentState;
        public UserActionState CurrentState
        {
            get { return _currentState; }
            private set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(CurrentState));

                _currentState = value;

                base.RaiseAllPropertiesChanged();
            }
        }

        public void AddState(string stateName, IUserAction action)
        {
            var state = new UserActionState(stateName)
            {
                Action = action
            };

            AddState(state);
        }

        public void AddState(UserActionState state)
        {
            States.Add(state.UniqueName, state);

            if (CurrentState == null)
                CurrentState = state;
        }

        public void SetState(string stateName)
        {
            CurrentState = States[stateName];
        }

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
