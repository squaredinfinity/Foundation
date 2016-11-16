using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Graphics.ColorSpaces
{
    public abstract class RGBColorSpace : IColorSpace
    {
        public abstract string Name
        {
            get;
        }

        public ColorChannelDefinition Alpha { get; private set; }
        public ColorChannelDefinition R { get; private set; }
        public ColorChannelDefinition G { get; private set; }
        public ColorChannelDefinition B { get; private set; }

        /// <summary>
        /// M Matrix which transfroms from XYZ space to RGB space
        /// </summary>
        protected abstract double[,] M_XYZToRGB { get; }

        /// <summary>
        /// M Matrix which transfroms from RGB space to XYZ space
        /// </summary>
        protected abstract double[,] M_RGBToXYZ { get; }

        public RGBColorSpace()
        {
            Alpha = new ColorChannelDefinition(this, "Alpha", "A", (v) => v % 255);
            R = new ColorChannelDefinition(this, "R", "R", (v) => v % 255);
            G = new ColorChannelDefinition(this, "G", "G", (v) => v % 255);
            B = new ColorChannelDefinition(this, "B", "B", (v) => v % 255);
        }


        protected abstract IColor GetColor(double alpha, double red, double green, double blue);

        public IColor FromXYZColor(XYZColor xyzColor)
        {
            var xyzVector = new double[3] { xyzColor.X.Value, xyzColor.Y.Value, xyzColor.Z.Value };

            var rgbVector = Multiply(M_XYZToRGB, xyzVector);

            return GetColor(xyzColor.Alpha.Value, rgbVector[0], rgbVector[1], rgbVector[2]);
        }

        public XYZColor ToXYZColor(IColor color)
        {
            var rgbColor = color as RGBColor;

            var rgbVector = new double[3] { rgbColor.R.Value, rgbColor.G.Value, rgbColor.B.Value };

            var xyzVector = Multiply(M_RGBToXYZ, rgbVector);

            return new XYZColor(color.Channels["Alpha"].Value, xyzVector[0], xyzVector[1], xyzVector[2]);
        }

        double[] Multiply(double[,] m, double[] channelsVector)
        {
            var rowCount = m.GetLength(0);
            var sumCount = m.GetLength(1);

            var result = new double[rowCount];

            Parallel.For(0, rowCount, r =>
            {
                for (int c = 0; c < sumCount; c++)
                {
                    //Trace.WriteLine(string.Format("[thread {0}] {1} += {2} * {3}, r:{4}, i:{5}", Thread.CurrentThread.ManagedThreadId, result[r], m[c, r], rgbVector[c], c, r));

                    result[r] += m[r, c] * channelsVector[c];
                }
            });

            return result;
        }
    }
}
