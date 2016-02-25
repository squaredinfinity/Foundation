using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SquaredInfinity.Foundation.Media.Drawing
{
    public abstract partial class PixelCanvas : IPixelCanvas
    {
        [Flags]
        public enum CohenSutherlandOutCode
        {
            Inside = 0, // 0000
            Left = 1,   // 0001
            Right = 2,  // 0010
            Bottom = 4, // 0100
            Top = 8     // 1000
        }

        /// <summary>
        /// https://en.wikipedia.org/wiki/Cohen%E2%80%93Sutherland_algorithm
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public CohenSutherlandOutCode ComputeCohenSutherlandOutCode(Rect bounds, double x, double y)
        {
            var code = CohenSutherlandOutCode.Inside;

            if (x < bounds.Left)
                code |= CohenSutherlandOutCode.Left;
            else if (x > bounds.Right)
                code |= CohenSutherlandOutCode.Right;

            if (y > bounds.Bottom)
                code |= CohenSutherlandOutCode.Bottom;
            else if (y < bounds.Top)
                code |= CohenSutherlandOutCode.Top;

            return code;
        }

        /// <summary>
        /// https://en.wikipedia.org/wiki/Cohen%E2%80%93Sutherland_algorithm
        /// </summary>
        public bool TryCohenSutherlandClip(Rect bounds, ref double x0, ref double y0, ref double x1, ref double y1)
        {
            var p0_outcode = ComputeCohenSutherlandOutCode(bounds, x0, y0);
            var p1_outcode = ComputeCohenSutherlandOutCode(bounds, x1, y1);

            // both points inside bounds, nothing to do here
            if (p0_outcode == CohenSutherlandOutCode.Inside && p1_outcode == CohenSutherlandOutCode.Inside)
                return true;

            bool result = false; // assume failed state

            while (true)
            {
                if ((p0_outcode | p1_outcode) == CohenSutherlandOutCode.Inside)
                {
                    result = true;
                    break;
                }
                else if ((p0_outcode & p1_outcode) != 0)
                {
                    break;
                }
                else
                {
                    double x, y;

                    // pick the code that is outside of the bounds
                    var outcode = p0_outcode != CohenSutherlandOutCode.Inside ? p0_outcode : p1_outcode;

                    // find intersection of the line with bounds

                    // y = y0 + slope * (x - x0)
                    // x = x0 + (1 / slope) * (y - y0)

                    // slope = (y1 - y0) / (x1 - x0)

                    if ((outcode & CohenSutherlandOutCode.Top) != 0)
                    {
                        // point above bounds, so y = bounds.Top
                        y = bounds.Top;
                        x = x0 + (x1 - x0) * (y - y0) / (y1 - y0);
                    }
                    else if ((outcode & CohenSutherlandOutCode.Bottom) != 0)
                    {
                        // point below bounds, so y = bounds.Bottom
                        y = bounds.Bottom;
                        x = x0 + (x1 - x0) * (y - y0) / (y1 - y0);
                    }
                    else if ((outcode & CohenSutherlandOutCode.Right) != 0)
                    {
                        // point is to the right, so x = bounds.Right
                        x = bounds.Right;
                        y = y0 + (y1 - y0) * (x - x0) / (x1 - x0);
                    }
                    else if ((outcode & CohenSutherlandOutCode.Left) != 0)
                    {
                        // point is to the left, so x = bounds.Left
                        x = bounds.Left;
                        y = y0 + (y1 - y0) * (x - x0) / (x1 - x0);
                    }
                    else
                    {
                        x = double.NaN;
                        y = double.NaN;
                    }

                    if (outcode == p0_outcode)
                    {
                        x0 = x;
                        y0 = y;
                        p0_outcode = ComputeCohenSutherlandOutCode(bounds, x0, y0);
                    }
                    else
                    {
                        x1 = x;
                        y1 = y;
                        p1_outcode = ComputeCohenSutherlandOutCode(bounds, x1, y1);
                    }
                }
            }

            return result;
        }
    }
}
