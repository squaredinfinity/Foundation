using SquaredInfinity.Foundation.Maths;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SquaredInfinity.Foundation.Graphics.Drawing
{
    public class PixelArrayCanvas : PixelCanvas
    {
        int[] _pixels;
        public int[] Pixels {  get { return _pixels; } }
        public override int[] GetPixels()
        {
            return _pixels;
        }

        public PixelArrayCanvas(int width, int height)
            : base(width, height)
        {
            _pixels = new int[width * height];
        }

        public override int this[int x, int y]
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

        public override int this[int position]
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

        public override int[] GetRow(int row)
        {
            var result = new int[_width];
            Array.Copy(_pixels, row * _width, result, 0, _width);

            return result;
        }

        public override void SetRow(int row, int[] pixels)
        {
            Array.Copy(pixels, 0, _pixels, row * _width, _width);
        }

        public override void Clear(int color)
        {
            // fill first line ..

            for (int w = 0; w < _width; w++)
            {
                this[w] = color;
            }

            // block copy first line to all remaining lines
            var line = 1;

            while (line < _height)
            {
                Array.ConstrainedCopy(_pixels, 0, _pixels, (_width * line), _width);
                line++;
            }
        }

        public override void ReplaceFromPixels(int[] pixels, int width, int height)
        {
            UpdateBounds(width, height);

            _pixels = pixels;
        }

        public override void Resize(int width, int height)
        {
            Array.Resize(ref _pixels, width * height);

            UpdateBounds(width, height);
        }
    }
}
