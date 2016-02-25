using SquaredInfinity.Foundation.Presentation.Controls.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.Foundation.Presentation.Infrastructure
{
    public abstract class SelectorBehaviorController : ISelectorBehaviorController
    {
        public virtual IReadOnlyList<IUserAction> GetAvailableUserAction(object selectorIdentifier, object item)
        {
            return new IUserAction[0];
        }

        public virtual void OnItemSelected(object selectorIdentifier, object selectedItem)
        { }

        public virtual void OnItemUnselected(object selectorIdentifier, object unselectedItem)
        { }
    }
}
