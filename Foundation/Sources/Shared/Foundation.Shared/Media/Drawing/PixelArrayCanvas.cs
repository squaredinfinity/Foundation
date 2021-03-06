﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Media.Drawing
{
    public class PixelArrayCanvas : PixelCanvas
    {
        int[] _pixels;
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
            Width = width;
            Height = height;
            Length = width * height;
            Stride = width * 4;
            Bounds = new Rectangle(0, 0, _width, _height);

            _pixels = pixels;
        }
    }
}
