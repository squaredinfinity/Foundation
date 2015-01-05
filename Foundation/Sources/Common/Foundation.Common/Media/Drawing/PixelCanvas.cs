using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Media.Drawing
{
    public partial class PixelCanvas : IPixelCanvas, IGdiPixelCanvas
    {
        int _length;
        /// <summary>
        /// Returns total number of pixels in this bitmap
        /// </summary>
        public int Length
        {
            get { return _length; }
            private set { _length = value; }
        }

        int _stride;
        /// <summary>
        /// There's not padding used and Stride always equals Width at the moment.
        /// </summary>
        public int Stride
        {
            get { return _stride; }
            private set { _stride = value; }
        }

        int _width;
        /// <summary>
        /// Width (in pixels) of the bitmap
        /// </summary>
        public int Width
        {
            get { return _width; }
            private set { _width = value; }
        }


        int _height;
        /// <summary>
        /// Height (in pixels) of the bitmap
        /// </summary>
        public int Height
        {
            get { return _height; }
            private set { _height = value; }
        }

        int[] _pixels;

        public PixelCanvas(int width, int height)
        {
            Width = width;
            Height = height;

            Stride = Width;

            Length = Stride * Height;

            _pixels = new int[width * height];
        }

        public int this[int x, int y]
        {
            get
            {
                return _pixels[y * _width + x];
            }
            set
            {
                _pixels[y * _width + x] = value;
            }
        }

        public int this[int position]
        {
            get
            {
                return _pixels[position];
            }
            set
            {
                _pixels[position] = value;
            }
        }

        public int[] GetRow(int row)
        {
            var result = new int[_width];
            Array.Copy(_pixels, row * _width, result, 0, _width);

            return result;
        }
        
        public void SetRow(int row, int[] pixels)
        {
            Array.Copy(pixels, 0, _pixels, row * _width, _width);
        }

        System.Drawing.Color IGdiPixelCanvas.GetColor(int color)
        {
            return Color.FromArgb(color);
        }

        int IGdiPixelCanvas.GetColor(System.Drawing.Color color)
        {
            return color.ToArgb();
        }
    }
}
