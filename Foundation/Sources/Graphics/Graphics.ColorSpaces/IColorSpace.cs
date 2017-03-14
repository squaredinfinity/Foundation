using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Graphics.ColorSpaces
{
    public interface IColorSpace
    {
        /// <summary>
        /// Name of this color space
        /// </summary>
        string Name { get; }

        IColor FromXYZColor(XYZColor xyzColor);
        XYZColor ToXYZColor(IColor color);
    }
}
