using SquaredInfinity.Foundation.Presentation.Controls.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.Foundation.Presentation.Infrastructure
{
    public interface ISelectorBehaviorController
    {
        void OnItemSelected(object selectorIdentifier, object selectedItem);
        void OnItemUnselected(object selectorIdentifier, object unselectedItem);
        IReadOnlyList<IUserAction> GetAvailableUserAction(object selectorIdentifier, object item);
    }
}
