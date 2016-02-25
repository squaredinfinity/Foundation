using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace SquaredInfinity.Foundation.Presentation.Controls.AdaptiveSelector
{
    public interface IAdaptiveSelectorItemGroup
    {
        string Name { get; }
        Color? Color { get; }
    }
}
