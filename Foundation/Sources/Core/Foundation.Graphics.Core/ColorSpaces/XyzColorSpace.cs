using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Foundation.Graphics.ColorSpaces
{
    /// <summary>
    /// CIE 1931 XYZ color space
    /// http://en.wikipedia.org/wiki/CIE_1931_color_space
    /// </summary>
    public sealed class XYZColorSpace : IColorSpace
    {
        public string Name
        {
            get { return "CIE 1931 XYZ (2°, D65)"; }
        }

        public ColorChannelDefinition Alpha { get; private set; }
        public ColorChannelDefinition X { get; private set; }
        public ColorChannelDefinition Y { get; private set; }
        public ColorChannelDefinition Z { get; private set; }

        readonly XYZColor whitePoint;
        /// <summary>
        /// The white reference illuminant, XYZ values scaled so that Y = 1 (2°, D65)
        /// </summary>
        public XYZColor WhitePoint
        {
            get { return whitePoint; }
        }

        internal XYZColorSpace()
        {
            Alpha = new ColorChannelDefinition(this, "Alpha", "A");

            // DisplayValue = Value * 100 and up to three fraction digits
            X = new ColorChannelDefinition(this, "X", "X", (v) => Math.Round(100 * v, 3, MidpointRounding.AwayFromZero));
            Y = new ColorChannelDefinition(this, "Y", "Y", (v) => Math.Round(100 * v, 3, MidpointRounding.AwayFromZero));
            Z = new ColorChannelDefinition(this, "Z", "Z", (v) => Math.Round(100 * v, 3, MidpointRounding.AwayFromZero));

            whitePoint = new XYZColor(this, 1.0, 0.95047, 1.0, 1.088969);
        }
        
        public IColor FromXYZColor(XYZColor xyzColor)
        {
            return xyzColor;
        }

        public XYZColor ToXYZColor(IColor color)
        {
            return (XYZColor)color;
        }
    }
}
