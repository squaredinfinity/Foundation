using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
        Rect Bounds { get;  }

        int[] GetPixels();

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

        void DrawLine(int x1, int y1, int x2, int y2, int color);
        void DrawLineDDA(int x1, int y1, int x2, int y2, int color);
        void DrawLineDDA(Rect bounds, int x1, int y1, int x2, int y2, int color);

        /// <summary>
        /// Draws smooth line using Wu Algorithm
        /// </summary>
        void DrawLineWu(int x1, int y1, int x2, int y2, int color);
        void DrawLineWu(int x1, int y1, int x2, int y2, int color, int width);
        void DrawLineWu(Rect bounds, int x1, int y1, int x2, int y2, int color);
        void DrawLineWu(Rect bounds, int x1, int y1, int x2, int y2, int color, int width);

        void Clear(int color);
        void Clear();

        int GetColor(int a, int r, int g, int b);

        void Blit(IPixelCanvas source, BlendMode blendMode);

        void Blit(
            Rect destination_rect,
            IPixelCanvas source,
            Rect source_rect,
            BlendMode blendMode
            );

        void Blit(
            Rect destination_rect,
            IPixelCanvas source,
            Rect source_rect,
            byte alpha,
            byte red,
            byte green,
            byte blue,
            BlendMode blendMode);

        bool IntersectsWith(Rect rect);

        void ReplaceFromPixels(int[] pixels, int width, int height);
    }
}
