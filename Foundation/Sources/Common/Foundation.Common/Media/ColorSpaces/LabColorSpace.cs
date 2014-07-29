using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Foundation.Media.ColorSpaces
{
    public sealed class LabColorSpace : IColorSpace
    {
        public string Name
        {
            get { return "CIE 1976 Lab) [CIELAB]"; }
        }

        public ColorChannelDefinition Alpha { get; private set; }
        public ColorChannelDefinition Lightness { get; private set; }
        public ColorChannelDefinition a { get; private set; }
        public ColorChannelDefinition b { get; private set; }
    
        internal LabColorSpace()
        {
            Alpha = new ColorChannelDefinition(this, "Alpha", "A");
            Lightness = new ColorChannelDefinition(this, "Lightness", "L");
            a = new ColorChannelDefinition(this, "a", "a");
            b = new ColorChannelDefinition(this, "b", "b");
        }

        /// <summary>
        /// 0.206896
        /// </summary>
        public const double Sigma = 6 / 29;

        /// <summary>
        /// 0.008856
        /// </summary>
        public readonly double CubicSigma = Math.Pow(Sigma, 3);

        /// <summary>
        /// 7.787037 = (1/3 * (29/6)^2
        /// </summary>
        public readonly double ToLabConversionConstant = Math.Pow(29 / 6, 2) / 3;

        /// <summary>
        /// 0.128418
        /// </summary>
        public readonly double ToXYZConversionConstant = 3 * Math.Pow(Sigma, 2);

        public IColor FromXYZColor(XYZColor xyzColor)
        {
            /// conversion algorithm created with help of http://en.wikipedia.org/wiki/Lab_color_space
            
            var fx = f_forward_conversion(xyzColor.X, KnownColorSpaces.XYZ.WhitePoint.X);
            var fy = f_forward_conversion(xyzColor.Y, KnownColorSpaces.XYZ.WhitePoint.Y);
            var fz = f_forward_conversion(xyzColor.Z, KnownColorSpaces.XYZ.WhitePoint.Z);

            var L = 116 * fy - 16;
            var a = 500 * (fx - fy);
            var b = 200 * (fy - fz);

            return new LabColor(xyzColor.Alpha, L, a, b);
        }

        public XYZColor ToXYZColor(IColor color)
        {
            /// conversion algorithm created with help of http://en.wikipedia.org/wiki/Lab_color_space

            var labColor = color as LabColor;

            var l = (labColor.L + 16) / 116;

            var fy = f_reverse_conversion(l);
            var fx = f_reverse_conversion(l + labColor.a / 500);
            var fz = f_reverse_conversion(l - labColor.b / 200);

            var y = KnownColorSpaces.XYZ.WhitePoint.Y * fy;
            var x = KnownColorSpaces.XYZ.WhitePoint.X * fx;
            var z = KnownColorSpaces.XYZ.WhitePoint.Z * fz;

            return new XYZColor(labColor.Alpha, x, y, z);            
        }

        /// <summary>
        /// transformation function used for conversion from XYZ to LAB
        /// </summary>
        /// <param name="t">value of X, Y or Z color channel</param>
        /// <param name="tn">value of white point of X, Y or Z color channel</param>
        /// <returns></returns>
        double f_forward_conversion(double t, double tn)
        {
            var r = t / tn;

            //! there is a division of the f function into two domains. It was done to prevent an infinity slope at x = 0.
            //! f(x) was assumed to be linear below some x = x0, and was assumed to match the x^(1/3) part of the function at x0 in both value and slope
            //! t0^(1/3) = at0 + b
            //! t0 = sigma^3

            r = r > CubicSigma ?
                Math.Pow(r, 1 / 3.0)
                : r * ToLabConversionConstant + (4.0 / 29.0);

            return r;
        }

        /// <summary>
        /// transformation function used for conversion from LAB to XYZ
        /// </summary>
        double f_reverse_conversion(double t)
        {
            var r = t > Sigma ?
                Math.Pow(t, 3)
                : ToXYZConversionConstant * (t - -(4.0 / 29.0));

            return r;
        }
    }
}
