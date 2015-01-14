using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Media.Drawing
{
    public unsafe partial class UnsafePixelCanvas : IPixelCanvas
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

        public int[] Pixels
        {
            get
            {
                var result = new int[_length];

                Marshal.Copy(new IntPtr((void*)(pixelsPointer)), result, 0, _length);

                return result;
            }
        }

        Bitmap image;
        
        PixelFormat imagePixelFormat;
        string fileName;

        BitmapData imageData;
        int* pixelsPointer;

        public UnsafePixelCanvas(int width, int height)
        {
            Width = width;
            Height = height;

            Stride = Width;

            Length = Stride * Height;

            imageData = new BitmapData();
            imageData.Width = Width;
            imageData.Height = Height;
            imageData.PixelFormat = System.Drawing.Imaging.PixelFormat.Format32bppArgb;
            imageData.Stride = Width;

            pixelsPointer = (int*)imageData.Scan0;
        }

        /// <summary>
        /// Pixel format used by the bitmap.
        /// It's always 32bppArgb
        /// </summary>
        public PixelFormat PixelFormat
        {
            get { return System.Drawing.Imaging.PixelFormat.Format32bppArgb; }
        }

        public int this[int x, int y]
        {
            get
            {
                return *(int*)(pixelsPointer + y * _width + x);
            }
            set
            {
                *(int*)(pixelsPointer + y * _width + x) = value;
            }
        }

        public int this[int position]
        {
            get
            {
                return *(int*)(pixelsPointer + position);
            }
            set
            {
                *(int*)(pixelsPointer + position) = value;
            }
        }

        public int[] GetRow(int row)
        {
            var result = new int[_width];

            Marshal.Copy(new IntPtr((void*)(pixelsPointer + row * _width)), result, 0, _width);

            return result;
        }

        public void SetRow(int row, int[] pixels)
        {
            Marshal.Copy(pixels, 0, new IntPtr((void*)(pixelsPointer + row * _width)), _width);
        }


        public System.Drawing.Color GetColor(int color)
        {
            return Color.FromArgb(color);
        }

        public int GetColor(System.Drawing.Color color)
        {
            return color.ToArgb();
        }
    }
}
