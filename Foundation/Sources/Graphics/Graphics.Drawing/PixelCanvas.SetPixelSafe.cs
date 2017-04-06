using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Graphics.Drawing
{
    public partial class
#if UNSAFE
        UnsafePixelCanvas
#else
 PixelCanvas
#endif
    {
        // NOTE:
        //      Those methods are supposed to be convinient but also as quick as possible.
        //      Some code duplication will occur.

        #region Color

        public void SetPixelSafe(int x, int y, int color)
        {
            if (x < 0 || y < 0 || x > _width || y > _height)
                return;

            this[x, y] = color;
        }

        public void SetPixelSafeAlpha(int x, int y, int color)
        {
            if (x < 0 || y < 0 || x > _width || y > _height)
                return;

            // destination pixel
            var dp = this[x, y];

            var da = ((dp >> 24) & 0xff);
            var dr = ((dp >> 16) & 0xff);
            var dg = ((dp >> 8) & 0xff);
            var db = ((dp) & 0xff);

            var sa = ((color >> 24) & 0xff);
            var sr = ((color >> 16) & 0xff);
            var sg = ((color >> 8) & 0xff);
            var sb = ((color) & 0xff);

            var isa = 255 - sa;

            dp =
                (((((sa << 8) + isa * da) >> 8) & 0xff) << 24) |
                (((((sr << 8) + isa * dr) >> 8) & 0xff) << 16) |
                (((((sg << 8) + isa * dg) >> 8) & 0xff) << 8) |
                ((((sb << 8) + isa * db) >> 8) & 0xff);

            this[x, y] = dp;
        }

        public void SetPixelSafe(int x, int y, int color, BlendMode blendMode)
        {
            if (x < 0 || y < 0 || x > _width || y > _height)
                return;

            if (blendMode == BlendMode.Copy)
            {
                this[x, y] = color;
            }
            else if (blendMode == BlendMode.Alpha)
            {
                // destination pixel
                var dp = this[x, y];

                var da = ((dp >> 24) & 0xff);
                var dr = ((dp >> 16) & 0xff);
                var dg = ((dp >> 8) & 0xff);
                var db = ((dp) & 0xff);

                var sa = ((color >> 24) & 0xff);
                var sr = ((color >> 16) & 0xff);
                var sg = ((color >> 8) & 0xff);
                var sb = ((color) & 0xff);

                var isa = 255 - sa;

                dp =
                    (((((sa << 8) + isa * da) >> 8) & 0xff) << 24)  |
                    (((((sr << 8) + isa * dr) >> 8) & 0xff) << 16)  |
                    (((((sg << 8) + isa * dg) >> 8) & 0xff) << 8)   |
                    ((((sb << 8) + isa * db) >> 8) & 0xff);

                this[x, y] = dp;
            }
            else throw new NotSupportedException(blendMode.ToString());
        }

        #endregion

        #region Color Components

        public void SetPixelSafe(int x, int y, int alpha, int red, int green, int blue)
        {
            if (x < 0 || y < 0 || x > _width || y > _height)
                return;

            var dp =
                (alpha << 24) |
                (red << 16) |
                (green << 8) |
                (blue >> 8);

            this[x, y] = dp;
        }

        public void SetPixelSafeAlpha(int x, int y, int alpha, int red, int green, int blue, int isa)
        {
            if (x < 0 || y < 0 || x > _width || y > _height)
                return;

            if (alpha == 0)
                return;

            // destination pixel
            var dp = this[x, y];

            var da = ((dp >> 24) & 0xff);
            var dr = ((dp >> 16) & 0xff);
            var dg = ((dp >> 8) & 0xff);
            var db = ((dp) & 0xff);

            dp =
                (((((alpha << 8) + isa * da) >> 8) & 0xff) << 24) |
                (((((red << 8) + isa * dr) >> 8) & 0xff) << 16) |
                (((((green << 8) + isa * dg) >> 8) & 0xff) << 8) |
                ((((blue << 8) + isa * db) >> 8) & 0xff);

            this[x, y] = dp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="alpha"></param>
        /// <param name="red"></param>
        /// <param name="green"></param>
        /// <param name="blue"></param>
        /// <param name="isa">255 - source alpha</param>
        /// <param name="blendMode"></param>
        public void SetPixelSafe(int x, int y, int alpha, int red, int green, int blue, int isa, BlendMode blendMode)
        {
            if (x < 0 || y < 0 || x > _width || y > _height)
                return;

            if (blendMode == BlendMode.Copy || alpha == 255)
            {
                var dp =
                    (alpha << 24)   |
                    (red << 16)     |
                    (green << 8)    |
                    (blue >> 8);

                this[x, y] = dp;
            }
            else if (blendMode == BlendMode.Alpha)
            {
                if (alpha == 0)
                    return;

                // destination pixel
                var dp = this[x, y];

                var da = ((dp >> 24) & 0xff);
                var dr = ((dp >> 16) & 0xff);
                var dg = ((dp>> 8) & 0xff);
                var db = ((dp) & 0xff);

                dp =
                    (((((alpha << 8) + isa * da) >> 8) & 0xff) << 24)   | 
                    (((((red << 8) + isa * dr) >> 8) & 0xff) << 16)     | 
                    (((((green << 8) + isa * dg) >> 8) & 0xff) << 8)    | 
                    ((((blue << 8) + isa * db) >> 8) & 0xff);

                this[x, y] = dp;
            }
            else throw new NotSupportedException(blendMode.ToString());
        }

        #endregion

        #region Intensity

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="alpha"></param>
        /// <param name="red"></param>
        /// <param name="green"></param>
        /// <param name="blue"></param>
        /// <param name="isa">255 - alpha</param>
        /// <param name="intensity"></param>
        /// <param name="blendMode"></param>
        public void SetPixelSafe(int x, int y, int alpha, int red, int green, int blue, int isa, int intensity, BlendMode blendMode)
        {
            if (x < 0 || y < 0 || x > _width || y > _height)
                return;

            if (blendMode == BlendMode.Copy || alpha == 255)
            {
                if (intensity != 1)
                {
                    alpha = ((alpha * intensity) >> 8);
                    red = ((red * intensity) >> 8);
                    green = ((green * intensity) >> 8);
                    blue = ((blue * intensity) >> 8);
                }

                var dp = 
                    (alpha << 24)   |
                    (red << 16)     |
                    (green << 8)    | 
                    (blue >> 8);

                this[x, y] = dp;
            }
            else if (blendMode == BlendMode.Alpha)
            {
                if (alpha == 0)
                    return;

                var dp = this[x, y];

                var da = ((dp >> 24) & 0xff);
                var dr = ((dp >> 16) & 0xff);
                var dg = ((dp >> 8) & 0xff);
                var db = ((dp) & 0xff);

                if (intensity != 1)
                {
                    alpha = ((alpha * intensity) >> 8);
                    red = ((red * intensity) >> 8);
                    green = ((green * intensity) >> 8);
                    blue = ((blue * intensity) >> 8);

                    isa = 255 - alpha;
                }

                dp = 
                    ((((((alpha) << 8) + isa * da) >> 8) & 0xff) << 24) |
                    ((((((red) << 8) + isa * dr) >> 8) & 0xff) << 16)   |
                    ((((((green) << 8) + isa * dg) >> 8) & 0xff) << 8)  |
                    (((((blue) << 8) + isa * db) >> 8) & 0xff);

                this[x, y] = dp;
            }
            else throw new NotSupportedException(blendMode.ToString());
        }

        #endregion
    }
}