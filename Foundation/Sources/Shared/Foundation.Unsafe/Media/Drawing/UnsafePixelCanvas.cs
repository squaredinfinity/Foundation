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
    public unsafe partial class UnsafePixelCanvas : PixelCanvas
    {
        public override int[] GetPixels()
        {
                throw new NotImplementedException();
                //return (int[])(int*[]) pixelsPointer;
        }

        Bitmap image;
        
        PixelFormat imagePixelFormat;
        string fileName;

        BitmapData imageData;
        int* pixelsPointer;

        public UnsafePixelCanvas(int width, int height)
            : base(width, height)
        {
            imageData = new BitmapData();
            imageData.Width = Width;
            imageData.Height = Height;
            imageData.PixelFormat = System.Drawing.Imaging.PixelFormat.Format32bppArgb;
            imageData.Stride = Width;

            pixelsPointer = (int*)imageData.Scan0;

            _bounds = new Rectangle(0, 0, _width, _height);
        }

        /// <summary>
        /// Pixel format used by the bitmap.
        /// It's always 32bppArgb
        /// </summary>
        public PixelFormat PixelFormat
        {
            get { return System.Drawing.Imaging.PixelFormat.Format32bppArgb; }
        }

        public override int this[int x, int y]
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

        public override int this[int position]
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

        public override int[] GetRow(int row)
        {
            var result = new int[_width];

            Marshal.Copy(new IntPtr((void*)(pixelsPointer + row * _width)), result, 0, _width);

            return result;
        }

        public override void SetRow(int row, int[] pixels)
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

        public bool IntersectsWith(System.Drawing.Rectangle rect)
        {
            return _bounds.IntersectsWith(rect);
        }

        public override void Clear(int color)
        {

        }

        public override void ReplaceFromPixels(int[] pixels, int width, int height)
        {
            throw new NotImplementedException();
        }
    }
}
