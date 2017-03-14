using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SquaredInfinity.Foundation.Graphics
{
    public class GraphicsUtility
    {
        static readonly double CentimetersPerInch = 2.54;

        public static double DpiX { get; private set; }
        public static double DpcX
        {
            get
            {
                return DpiX * CentimetersPerInch;
            }
        }
        public static double DpiY { get; private set; }
        public static double DpcY
        {
            get
            {
                return DpiY * CentimetersPerInch;
            }
        }

        static GraphicsUtility()
        {
            DpiX = 96.0;
            DpiY = 96.0;
        }

        public static void UpdateDpi(double dpix, double dpiy)
        {
            DpiX = dpix;
            DpiY = dpiy;
        }

        public static double MilimetersToPixelsX(double milimeters)
        {
            return (milimeters / 10) * DpiX / CentimetersPerInch;
        }

        public static double MilimetersToPixelsY(double milimeters)
        {
            return (milimeters / 10) * DpiY / CentimetersPerInch;
        }

        public static double CentimitersToPixelsX(double centimiters)
        {
            return centimiters * DpiX / CentimetersPerInch;
        }

        public static double CentimitersToPixelsY(double centimiters)
        {
            return centimiters * DpiY / CentimetersPerInch;
        }

        public static double PixelsToCentimitersX(double pixels)
        {
            return pixels * CentimetersPerInch / DpiX;
        }

        public static double PixelsToCentimitersY(double pixels)
        {
            return pixels * CentimetersPerInch / DpiY;
        }
    }
}
