﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Media.Drawing
{
    /// <summary>
    /// Provides a fast way to create and modify pixels.
    /// Contains array of pixels each containing ARGB color value stored as int32 for best performance
    /// </summary>
    public interface IPixelCanvas : IDisposable
    {
        int Length { get; }
        int Stride { get; }
        int Width { get; }
        int Height { get; }

        int[] Pixels { get; }

        /// <summary>
        /// Gets or sets color of a pixel at a give position
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        int this[int position] { get; set; }

        /// <summary>
        /// Gets or sets color of a pixel at given coordinates
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        int this[int x, int y] { get; set; }


        int[] GetRow(int row);
        void SetRow(int row, int[] pixels);

        System.Drawing.Color GetColor(int color);

        int GetColor(System.Drawing.Color color);

        void DrawLineDDA(int x1, int y1, int x2, int y2, int color);
    }
}