using SquaredInfinity.Graphics.ColorSpaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Graphics.ColorSpaces
{
    public interface IColor
    {
        /// <summary>
        /// Returns the color space for which this color is defined.
        /// </summary>
        IColorSpace ColorSpace { get; }

        ColorChannelCollection Channels { get; }
    }
}
