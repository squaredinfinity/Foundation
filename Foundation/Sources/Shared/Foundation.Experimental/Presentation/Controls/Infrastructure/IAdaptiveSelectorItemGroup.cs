using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace SquaredInfinity.Presentation.Controls
{
    public interface IAdaptiveSelectorItemGroup : IEquatable<IAdaptiveSelectorItemGroup>
    {
        string UniqueName { get; }
        string DisplayName { get; }
        Color? Color { get; }
    }
}
