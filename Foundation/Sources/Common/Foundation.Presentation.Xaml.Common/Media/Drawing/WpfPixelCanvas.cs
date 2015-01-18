using SquaredInfinity.Foundation.Media.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SquaredInfinity.Foundation.Presentation.Media.Drawing
{
    public class WpfPixelCanvas : PixelCanvas
    {
        Int32Rect _boundsAsInt32Rect;
        /// <summary>
        /// Used as a parameter for Writeable Bitmap
        /// </summary>
        public Int32Rect BoundsAsInt32Rect
        {
            get { return _boundsAsInt32Rect; }
            private set { _boundsAsInt32Rect = value; }
        }

        public WpfPixelCanvas(int width, int height)
            : base (width, height)
        {
            _boundsAsInt32Rect = new Int32Rect(0, 0, width, height);
        }
        
        public WriteableBitmap ToWriteableBitmap()
        {
            var wb = new WriteableBitmap(Width, Height, 96, 96, PixelFormats.Pbgra32, palette: null);

            wb.WritePixels(BoundsAsInt32Rect, Pixels, Stride, 0);

            return wb;
        }

        public WriteableBitmap ToFrozenWriteableBitmap()
        {
            var wb = ToWriteableBitmap();

            wb.Freeze();

            return wb;
        }

        public void DrawLine(int x1, int y1, int x2, int y2, System.Windows.Media.Color color)
        {
            base.DrawLineDDA(x1, y1, x2, y2, GetColor(color));
        }

        public void DrawLineDDA(int x1, int y1, int x2, int y2, System.Windows.Media.Color color)
        {
            base.DrawLineDDA(x1, y1, x2, y2, GetColor(color));
        }

        public int GetColor(System.Windows.Media.Color color)
        {
            // #    Overview
            //      ARGB channels have 1 byte each
            //      RGB channels are pre-multiplied by A, to improve performance of certain operations that use transparency

#if NOT_OPTIMIZED

            //# Below is a non-optimized version of conversion algorithm

            // get Alpha value as a fraction of maximum channel value
            var a_fraction = color.A / 255;

            // construct final 32bit color by moving each channel value to appropriate place
            // BITS:        xxxx|xxxx|xxxx|xxxx
            // CHANNELS:      A | R' | G' | B'      (note: R',G',B' are pre-multiplied channels)
            return
                (color.A << 24)
                // R,G,B channels must be premultiplied by Alpha
                | (color.R * a_fraction) << 16
                | (color.G * a_fraction) << 8
                | (color.B * a_fraction);
#else
            // NOTE:    This is an optimized version of algorithm above
            //          It avoids floating point arithmetics for faster execution
            //          In tests it's about 2x faster

            var a = color.A + 1;

            // alpha is 0, pre-multiplied channels will also be 0
            if (a == 0)
                return 0;

            // alpha is 255, just copy channels values
            if(a == 255)
                return (255 << 24)
                    // shift right by 8 bits to keep only most significant byte of the multiplication result
                    | color.R << 16
                    | color.G << 8
                    | color.B;

            return
                (a << 24)
                // R,G,B channels must be premultiplied by Alpha
                // shift right by 8 bits to keep only most significant byte of the multiplication result
                | (color.R * a) << 16
                | (color.G * a) << 8
                | (color.B * a);
#endif
        }
    }
}
