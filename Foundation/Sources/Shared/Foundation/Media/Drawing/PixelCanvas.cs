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
        protected int _length;
        /// <summary>
        /// Returns total number of pixels in this bitmap
        /// </summary>
        public int Length
        {
            get { return _length; }
            protected set { _length = value; }
        }

        protected int _stride;
        /// <summary>
        /// There's not padding used and Stride always equals Width at the moment.
        /// </summary>
        public int Stride
        {
            get { return _stride; }
            protected set { _stride = value; }
        }

        protected int _width;
        /// <summary>
        /// Width (in pixels) of the bitmap
        /// </summary>
        public int Width
        {
            get { return _width; }
            protected set { _width = value; }
        }


        protected int _height;
        /// <summary>
        /// Height (in pixels) of the bitmap
        /// </summary>
        public int Height
        {
            get { return _height; }
            protected set { _height = value; }
        }

        public abstract int[] GetPixels();

        protected Rect _bounds;
        public Rect Bounds
        {
            get { return _bounds; }
            protected set { _bounds = value; }
        }
        
        public PixelCanvas(int width, int height)
        {
            Width = width;
            Height = height;

            Stride = Width * 4;

            Length = Width * Height;

            _bounds = new Rect(0, 0, _width, _height);
        }

        public abstract int this[int x, int y] { get; set; }

        public abstract int this[int position] {  get; set; }

        public abstract int[] GetRow(int row);

        public abstract void SetRow(int row, int[] pixels);

        public int GetColor(int a, int r, int g, int b)
        {
            // #    Overview
            //      ARGB channels have 1 byte each
            //      RGB channels are pre-multiplied by A, to improve performance of certain operations that use transparency

#if NOT_OPTIMIZED

            //# Below is a non-optimized version of conversion algorithm

            // get Alpha value as a fraction of maximum channel value
            var a_fraction = a / 255;

            // construct final 32bit color by moving each channel value to appropriate place
            // BITS:        xxxx|xxxx|xxxx|xxxx
            // CHANNELS:      A | R' | G' | B'      (note: R',G',B' are pre-multiplied channels)
            return
                (a << 24)
                // R,G,B channels must be premultiplied by Alpha
                | (r * a_fraction) << 16
                | (g * a_fraction) << 8
                | (b * a_fraction);
#else
            // NOTE:    This is an optimized version of algorithm above
            //          It avoids floating point arithmetics for faster execution
            //          In tests it's about 2x faster

            // alpha is 0, pre-multiplied channels will also be 0
            if (a == 0)
                return 0;

            // alpha is 255, just copy channels values
            if (a == 255)
                return (255 << 24)
                    // shift right by 8 bits to keep only most significant byte of the multiplication result
                    | r << 16
                    | g << 8
                    | b;

            var ai = a + 1;

            return
                (a << 24)
                // R,G,B channels must be premultiplied by Alpha
                // shift right by 8 bits to keep only most significant byte of the multiplication result
                | ((r * ai) >> 8) << 16
                | ((g * ai) >> 8) << 8
                | ((b * ai) >> 8);
#endif
        }

        public bool IntersectsWith(Rect rect)
        {
            return _bounds.IntersectsWith(rect);
        }

        public abstract void ReplaceFromPixels(int[] pixels, int width, int height);
    }
}
