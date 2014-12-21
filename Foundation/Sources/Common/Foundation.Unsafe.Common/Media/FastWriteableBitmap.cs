using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Media
{
    public unsafe partial class FastWriteableBitmap : IFastWriteableBitmap, IGdiFastWriteableBitmap
    {
        Bitmap image;
        
        PixelFormat imagePixelFormat;
        string fileName;

        BitmapData imageData;
        int* pixelsPointer;

        public FastWriteableBitmap(int width, int height)
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

        /// <summary>
        /// Pixel format used by the bitmap.
        /// It's always 32bppArgb
        /// </summary>
        public PixelFormat PixelFormat
        {
            get { return System.Drawing.Imaging.PixelFormat.Format32bppArgb; }
        }

        //public Bitmap Image
        //{
        //    get
        //    {
        //        if (!disposed)
        //        {
        //            if (image != null)
        //            {
        //                image.UnlockBits(imageData);
        //                Bitmap returnValue = new Bitmap(image);
        //                imageData = image.LockBits(new Rectangle(0, 0, imageWidth, imageHeight), ImageLockMode.ReadWrite, imagePixelFormat);

        //                return returnValue;
        //            }
        //            else
        //            {
        //                throw new InvalidOperationException("cannot copy Image because the Image property has not been set yet");
        //            }
        //        }
        //        else
        //        {
        //            throw new ObjectDisposedException("ImageTraverser");
        //        }
        //    }

        //    set
        //    {
        //        if (!disposed)
        //        {
        //            if (image != null)
        //            {
        //                fileName = "";
        //                baseImagePtr = (int*)IntPtr.Zero.ToPointer();
        //                image.UnlockBits(imageData);
        //                image.Dispose();

        //                imageData = null;
        //                image = null;
        //            }

        //            if (value != null)
        //            {
        //                image = new Bitmap(value);
        //                _size = image.Size;
        //                imageWidth = _size.Width;
        //                imageHeight = _size.Height;
        //                imagePixelFormat = image.PixelFormat;

        //                imageData = image.LockBits(new Rectangle(0, 0, imageWidth, imageHeight), ImageLockMode.ReadWrite, imagePixelFormat);
        //                baseImagePtr = (int*)imageData.Scan0.ToPointer();
        //                _stride = imageData.Stride;
        //                //bytesPerPixel = Bitmap.GetPixelFormatSize(imagePixelFormat) / 8;
        //                //offset = imageStride - imageWidth * bytesPerPixel;

        //                System.Diagnostics.Debug.Assert(imageData.Stride == imageWidth * 4);
        //            }
        //        }
        //        else
        //        {
        //            throw new ObjectDisposedException("ImageTraverser");
        //        }
        //    }
        //}

        /// <summary>
        /// Returns two dimensional array of pixels [row][column] where row_max = Height - 1, column_max = Width - 1
        /// </summary>
        /// <returns></returns>
        public int[][] ToArray2D()
        {
            var result = new int[Height][];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = GetRow(i);
            }

            return result;
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


        Color IGdiFastWriteableBitmap.GetPixel(int x, int y)
        {
            return Color.FromArgb(this[x, y]);
        }

        void IGdiFastWriteableBitmap.SetPixel(int x, int y, Color color)
        {
            this[x, y] = color.ToArgb();
        }
    }
}
