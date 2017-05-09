using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Primitives.NuGet
{
    public enum ListItemSelectionBehavior
    {
        /// <summary>
        /// Selection is disabled
        /// </summary>
        Disabled = 0,
        /// <summary>
        /// At least one item must be selected at any time
        /// </summary>
        MustHaveSelection = 1,
        /// <summary>
        /// Allows toggle of the selection.
        /// When selected item is clicked, it becomes unselected
        /// </summary>
        AllowToggle = 2,
        /// <summary>
        /// Single selection.
        /// Only one item can be selected at any point
        /// </summary>
        Single = 4,
        /// <summary>
        /// Multi selection.
        /// Multiple items can be selected when Ctrl is pressed
        /// </summary>
        Multi = 8,
        /// <summary>
        /// Multi selection.
        /// Multiple items can be selected just by clicking on them, Ctrl does not need to be pressed
        /// </summary>
        MutliWithoutCtrl = 16,

        SingleWithToggle = Single | AllowToggle,
        MultiWithToggle = Multi | AllowToggle
    }
}
