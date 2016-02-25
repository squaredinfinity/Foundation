using SquaredInfinity.Foundation.Presentation.Controls.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace SquaredInfinity.Foundation.Presentation.Controls.AdaptiveSelector
{
    /// <summary>
    /// An interface which can (but does not have to) be implemented by an item in this selector.
    /// Implementing it allows better control and customization of selector behavior
    /// </summary>
    public interface IAdaptiveSelectorItem
    {
        string DisplayName { get; }
        IAdaptiveSelectorItemGroup Group { get; }

        object Item { get; }

        /// <summary>
        /// Returns a color of this item.
        /// </summary>
        /// <returns></returns>
        Color? GetBackgroundColor();

        IReadOnlyList<IUserAction> GetAvailableUserActions();
    }
}
