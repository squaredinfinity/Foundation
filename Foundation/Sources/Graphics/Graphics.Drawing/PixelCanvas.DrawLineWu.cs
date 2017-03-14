using SquaredInfinity.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SquaredInfinity.Graphics.Drawing
{
    public partial class
#if UNSAFE
        UnsafePixelCanvas
#else
 PixelCanvas
#endif
    {

        public void DrawLineWu(Rectangle bounds, int x1, int y1, int x2, int y2, int color)
        {
            var x1_d = (double)x1;
            var y1_d = (double)y1;
            var x2_d = (double)x2;
            var y2_d = (double)y2;

            if (!TryCohenSutherlandClip(bounds, ref x1_d, ref y1_d, ref x2_d, ref y2_d))
                return;

            DrawLineWu(x1, y1, x2, y2, color);
        }

        public void DrawLineWu(Rectangle bounds, int x1, int y1, int x2, int y2, int color, int width)
        {
            var x1_d = (double)x1;
            var y1_d = (double)y1;
            var x2_d = (double)x2;
            var y2_d = (double)y2;

            if (!TryCohenSutherlandClip(bounds, ref x1_d, ref y1_d, ref x2_d, ref y2_d))
                return;

            DrawLineWu((int)x1_d, (int)y1_d, (int)x2_d, (int)y2_d, color, width);
        }

        const ushort INTENSITY_BITS = 8;
        const short NUM_LEVELS = 1 << INTENSITY_BITS; // 256
        // mask used to compute 1-value by doing value XOR mask
        const ushort WEIGHT_COMPLEMENT_MASK = NUM_LEVELS - 1; // 255
        const ushort INTENSITY_SHIFT = (ushort)(16 - INTENSITY_BITS); // 8
        
        public void DrawLineWu(int x1, int y1, int x2, int y2, int color)
        {
            // Assume pixels are centered on their coordinates
            // lines are 1 pixel thick
            // shape of line-ends is not important

            // ideally we would draw line from one pixel to another
            // then calculat for each pixel on a way the % of it being filled by the line
            // and use that % to set individual pixels' brightness
            // this would be too slow though.


            ushort error_step, error_accumulator;
            short delta_x, delta_y, x_step;
            int tmp;

            // rearrange start and end points so that algorithm runs from top to bottom (in y axis, 0 is at the top, with y values increasing to the bottom)
            if (y1 > y2)
            {
                tmp = y1;
                y1 = y2;
                y2 = tmp;

                tmp = x1;
                x1 = x2;
                x2 = tmp;
            }

            var source_alpha = ((color >> 24) & 0xff);
            var source_red = ((color >> 16) & 0xff);
            var source_green = ((color >> 8) & 0xff);
            var source_blue = ((color) & 0xff);


            // TODO: at the moment no alpha blending (with background) will occur, but this should be added as an option


            //var destination_red = ((destPixel >> 16) & 0xff);
            //var destination_green = ((destPixel >> 8) & 0xff);
            //var destination_blue = ((destPixel) & 0xff);


            //    var isa = 255 - source_alpha;

            //    destPixel = ((destination_alpha & 0xff) << 24) |
            //                (((((source_red << 8) + isa * destination_red) >> 8) & 0xff) << 16) |
            //                (((((source_green << 8) + isa * destination_green) >> 8) & 0xff) << 8) |
            //                 ((((source_blue << 8) + isa * destination_blue) >> 8) & 0xff);


            // draw initial pixel at full intensity
            SetPixelSafe(x1, y1, color); // todo: blend source with dest, when option is added

            // previously we swapped points based on y position (from top to bottom)
            // now lets see what the relation of x positions is
            delta_x = (short)(x2 - x1);

            if (delta_x >= 0)
            {
                x_step = 1;
            }
            else
            {
                x_step = -1;
                delta_x = (short)-delta_x; // set delta_x to a positve value
            }


            // horizontal, vertical and diagonal lines are special
            // they pass through centers of every pixel and don't need to be treated in a speciall way

            delta_y = (short)(y2 - y1);
            if (delta_y == 0) // horizontal line
            {
                while (delta_x-- != 0)
                {
                    x1 += x_step;

                    SetPixelSafe(x1, y1, color); // todo: alpha blend
                }

                return;
            }

            if (delta_x == 0) // vertical line 
            {
                while (delta_y-- != 0)
                {
                    y1++;
                    SetPixelSafe(x1, y1, color); // todo: alpha blend
                }

                return;
            }

            if (delta_x == delta_y) // diagonal line
            {
                while (delta_x-- != 0)
                {
                    x1 += x_step;
                    y1++;
                    SetPixelSafe(x1, y1, color); // todo: alpha blend
                }
                return;
            }


            // all other cases (not horizontal, vertical or diagonal line)

            // set cumulative error to 0
            error_accumulator = 0;

            // is it Y-major line?
            if (delta_y > delta_x)
            {
                // Y-major line; calculate 16-bit fixed-point fractional part of a
                // pixel that X advances each time Y advances 1 pixel, truncating the
                // result so that we won't overrun the endpoint along the X axis 
                error_step = (ushort)(((ulong)delta_x << 16) / (ulong)delta_y); // calculate slope * 65536

                // draw pixels between first and last
                while (--delta_y != 0)
                {
                    // get new accumulated error value
                    var new_error_accumulator = error_accumulator;

                    // advance accumulated error to next position
                    error_accumulator += error_step;

                    if (error_accumulator <= new_error_accumulator)
                    {
                        // error accumulator overflowed, move x direction
                        x1 += x_step;
                    }

                    // move y direction
                    y1++;

                    // two pixels will be set
                    // with colors components adjusted by intensity settings
                    // the sum of intensity of two bordering pixels equals the intensity of the line's pixel

                    // calculate intensity using accumulated error
                    var intensity = (ushort)(error_accumulator >> INTENSITY_SHIFT);

                    int weight = intensity;

                    var weighted_color =
                        (((source_alpha * weight) >> 8) << 24) |
                        (((source_red * weight) >> 8) << 16) |
                        (((source_green * weight) >> 8) << 8) |
                        ((source_blue * weight) >> 8);

                    //var destination_color = GetColor(255, 255, 255, 255);

                    //// get destination pixel rgb values
                    //var destination_red = ((destination_color >> 16) & 0xff); // 255
                    //var destination_green = ((destination_color >> 8) & 0xff); // 255
                    //var destination_blue = ((destination_color) & 0xff); // 255

                    SetPixelSafe(x1 + x_step, y1, weighted_color); 

                    weight = intensity ^ WEIGHT_COMPLEMENT_MASK;

                    weighted_color =
                        (((source_alpha * weight) >> 8) << 24) |
                        (((source_red * weight) >> 8) << 16) |
                        (((source_green * weight) >> 8) << 8) |
                        ((source_blue * weight) >> 8);

                    SetPixelSafe(x1, y1, weighted_color);
                }
            }
            else // it is X-major line
            {
                // It's an X-major line; calculate 16-bit fixed-point fractional part of a
                // pixel that Y advances each time X advances 1 pixel, truncating the
                // result to avoid overrunning the endpoint along the X axis */
                error_step = (ushort)(((ulong)delta_y << 16) / (ulong)delta_x); // calculate slope * 65536

                // draw pixels between first and last
                while (--delta_x != 0)
                {
                    // get new accumulated error value
                    var new_error_accumulator = error_accumulator;

                    // advance accumulated error to next position
                    error_accumulator += error_step;

                    if (error_accumulator <= new_error_accumulator)
                    {
                        // error accumulator overflowed, move y direction
                        y1++;
                    }

                    // move x direction
                    x1 += x_step;

                    // two pixels will be set
                    // with colors components adjusted by intensity settings
                    // the sum of intensity of two bordering pixels equals the intensity of the line's pixel

                    // calculate intensity using accumulated error
                    var intensity = (ushort)(error_accumulator >> INTENSITY_SHIFT);

                    int weight = intensity;

                    var weighted_color =
                       (((source_alpha * weight) >> 8) << 24) |
                       (((source_red * weight) >> 8) << 16) |
                       (((source_green * weight) >> 8) << 8) |
                       ((source_blue * weight) >> 8);

                    this[x1, y1 + 1] = weighted_color;

                    weight = intensity ^ WEIGHT_COMPLEMENT_MASK;

                    weighted_color =
                       (((source_alpha * weight) >> 8) << 24) |
                       (((source_red * weight) >> 8) << 16) |
                       (((source_green * weight) >> 8) << 8) |
                       ((source_blue * weight) >> 8);

                    this[x1, y1] = weighted_color;
                }
            }

            // draw the end of the line
            this[x2, y2] = color; // todo: alpha blend
        }


        public void DrawLineWu(int x1, int y1, int x2, int y2, int color, int width)
        {
            if (width <= 0)
                throw new ArgumentOutOfRangeException("width");

            // index shift determines where the line should be drawn from based on its width (e.g. line with thickness 1 - no shift, thickness 2, -1px shift, thickness 3, -2px shift)
            var index_shift = (width - 1) * -1;

            var index_shift_terminator = index_shift + width;

            // width is translated into an n-th odd number, where sequence number is shifted down by 1, so that 1st odd number is 0 (not really, but treated as such) 2nd odd number is 1, 3rd is 3, 4th is 5 etc.
            //  1   -   -1 (treated as 0)
            //  2   -   1
            //  3   -   3
            //  4   -   5
            //  5   -   7
            // actual width = (2 * (width - 1)) - 1
            width = (2 * (width - 1)) - 1;

            ushort error_step, error_accumulator;
            short delta_x, delta_y, x_step;
            int tmp;

            // rearrange start and end points so that algorithm runs from top to bottom (in y axis, 0 is at the top, with y values increasing to the bottom)
            if (y1 > y2)
            {
                tmp = y1;
                y1 = y2;
                y2 = tmp;

                tmp = x1;
                x1 = x2;
                x2 = tmp;
            }

            var source_alpha = ((color >> 24) & 0xff);
            var source_red = ((color >> 16) & 0xff);
            var source_green = ((color >> 8) & 0xff);
            var source_blue = ((color) & 0xff);

            var source_half =
                (((source_alpha * 127) >> 8) << 24) |
                (((source_red * 127) >> 8) << 16) |
                (((source_green * 127) >> 8) << 8) |
                ((source_blue * 127) >> 8);


            // draw initial pixel at full intensity
            //this[x1, y1] = color; // todo: blend source with dest, when option is added

            // previously we swapped points based on y position (from top to bottom)
            // now lets see what the relation of x positions is
            delta_x = (short)(x2 - x1);

            if (delta_x >= 0)
            {
                x_step = 1;
            }
            else
            {
                x_step = -1;
                delta_x = (short)-delta_x; // set delta_x to a positve value
            }


            // horizontal, vertical and diagonal lines are special
            // they pass through centers of every pixel and don't need to be treated in a speciall way

            delta_y = (short)(y2 - y1);
            if (delta_y == 0) // horizontal line
            {
                while (delta_x-- >= 0)
                {
                    //      this[x1, y1 + index_shift] = source_half;

                    for (var y_shift = index_shift + 1; y_shift < index_shift_terminator; y_shift++)
                    {
                        this[x1, y1 + y_shift] = color; // todo: alpha blend
                    }

                    this[x1, y1 + index_shift_terminator] = source_half;

                    x1 += x_step;
                }

                return;
            }

            if (delta_x == 0) // vertical line 
            {
                while (delta_y-- >= 0)
                {
                    this[x1 + index_shift, y1] = source_half;

                    for (var x_shift = index_shift + 1; x_shift < index_shift_terminator; x_shift++)
                    {
                        this[x1 + x_shift, y1] = color; // todo: alpha blend
                    }

                    //       this[x1 + index_shift_terminator, y1] = source_half;

                    y1++;
                }

                return;
            }

            if (delta_x == delta_y) // diagonal line
            {
                while (delta_x-- >= 0)
                {
                    this[x1 + index_shift, y1] = source_half; // todo: alpha blend

                    for (var x_shift = index_shift + 1; x_shift < index_shift_terminator; x_shift++)
                    {
                        this[x1 + x_shift, y1] = color; // todo: alpha blend
                    }

                    //this[x1 + index_shift_terminator, y1] = source_half; // todo: alpha blend

                    x1 += x_step;
                    y1++;
                    //this[x1, y1] = color; // todo: alpha blend
                }
                return;
            }


            // all other cases (not horizontal, vertical or diagonal line)

            // set cumulative error to 0
            error_accumulator = 0;

            // is it Y-major line?
            if (delta_y > delta_x)
            {
                // Y-major line; calculate 16-bit fixed-point fractional part of a
                // pixel that X advances each time Y advances 1 pixel, truncating the
                // result so that we won't overrun the endpoint along the X axis 
                error_step = (ushort)(((ulong)delta_x << 16) / (ulong)delta_y); // calculate slope * 65536

                // draw pixels between first and last
                while (--delta_y != 0)
                {
                    // get new accumulated error value
                    var new_error_accumulator = error_accumulator;

                    // advance accumulated error to next position
                    error_accumulator += error_step;

                    if (error_accumulator <= new_error_accumulator)
                    {
                        // error accumulator overflowed, move x direction
                        x1 += x_step;
                    }

                    // move y direction
                    y1++;

                    // two pixels will be set
                    // with colors components adjusted by intensity settings
                    // the sum of intensity of two bordering pixels equals the intensity of the line's pixel

                    // calculate intensity using accumulated error
                    var intensity = (ushort)(error_accumulator >> INTENSITY_SHIFT);

                    int weight = intensity;

                    var weighted_color =
                        (((source_alpha * weight) >> 8) << 24) |
                        (((source_red * weight) >> 8) << 16) |
                        (((source_green * weight) >> 8) << 8) |
                        ((source_blue * weight) >> 8);

                    //var destination_color = GetColor(255, 255, 255, 255);

                    //// get destination pixel rgb values
                    //var destination_red = ((destination_color >> 16) & 0xff); // 255
                    //var destination_green = ((destination_color >> 8) & 0xff); // 255
                    //var destination_blue = ((destination_color) & 0xff); // 255


                    //this[x1 + x_step, y1] = weighted_color;
                    this[x1 + x_step, y1 - 1] = weighted_color;
                    this[x1 + x_step, y1] = color;

                    weight = intensity ^ WEIGHT_COMPLEMENT_MASK;

                    weighted_color =
                        (((source_alpha * weight) >> 8) << 24) |
                        (((source_red * weight) >> 8) << 16) |
                        (((source_green * weight) >> 8) << 8) |
                        ((source_blue * weight) >> 8);

                    //this[x1, y1] = weighted_color;
                    this[x1 + x_step, y1 + 1] = weighted_color;
                }
            }
            else // it is X-major line
            {
                // It's an X-major line; calculate 16-bit fixed-point fractional part of a
                // pixel that Y advances each time X advances 1 pixel, truncating the
                // result to avoid overrunning the endpoint along the X axis */
                error_step = (ushort)(((ulong)delta_y << 16) / (ulong)delta_x); // calculate slope * 65536

                // draw pixels between first and last
                while (--delta_x != 0)
                {
                    // get new accumulated error value
                    var new_error_accumulator = error_accumulator;

                    // advance accumulated error to next position
                    error_accumulator += error_step;

                    if (error_accumulator <= new_error_accumulator)
                    {
                        // error accumulator overflowed, move y direction
                        y1++;
                    }

                    // move x direction
                    x1 += x_step;

                    // two pixels will be set
                    // with colors components adjusted by intensity settings
                    // the sum of intensity of two bordering pixels equals the intensity of the line's pixel

                    // calculate intensity using accumulated error
                    var intensity = (ushort)(error_accumulator >> INTENSITY_SHIFT);

                    int weight = intensity;

                    var weighted_color =
                       (((source_alpha * weight) >> 8) << 24) |
                       (((source_red * weight) >> 8) << 16) |
                       (((source_green * weight) >> 8) << 8) |
                       ((source_blue * weight) >> 8);

                    this[x1, y1 + 1] = weighted_color;


                    // this
                    this[x1, y1] = color;

                    weight = intensity ^ WEIGHT_COMPLEMENT_MASK;

                    weighted_color =
                       (((source_alpha * weight) >> 8) << 24) |
                       (((source_red * weight) >> 8) << 16) |
                       (((source_green * weight) >> 8) << 8) |
                       ((source_blue * weight) >> 8);

                    //this[x1, y1] = weighted_color;

                    this[x1, y1 - 1] = weighted_color;
                }
            }

            // draw the end of the line
            this[x2, y2] = color; // todo: alpha blend
        }

        // NOTE: ok results for line, but rather unpleasing joins between line segments
        //public void plotLineWidth(int x0, int y0, int x1, int y1, int color, int wd)
        //{
        //    var source_alpha = ((color >> 24) & 0xff);
        //    var source_red = ((color >> 16) & 0xff);
        //    var source_green = ((color >> 8) & 0xff);
        //    var source_blue = ((color) & 0xff);

        //    int dx = Math.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
        //    int dy = Math.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
        //    int err = dx - dy, e2, x2, y2;                          /* error value e_xy */
        //    var ed = dx + dy == 0 ? 1 : Math.Sqrt((float)dx * dx + (float)dy * dy);

        //    for (wd = (wd + 1) / 2; ;)
        //    {                                   /* pixel loop */
        //        var intensity = (int)Math.Max(0, 255 * (Math.Abs(err - dx + dy) / ed - wd + 1));
        //        var weight = intensity ^ WEIGHT_COMPLEMENT_MASK;
        //        var weighted_color =
        //               (((source_alpha * weight) >> 8) << 24) |
        //               (((source_red * weight) >> 8) << 16) |
        //               (((source_green * weight) >> 8) << 8) |
        //               ((source_blue * weight) >> 8);
        //        this[x0, y0] = weighted_color;

        //        e2 = err; x2 = x0;
        //        if (2 * e2 >= -dx)
        //        {                                           /* x step */
        //            for (e2 += dy, y2 = y0; e2 < ed * wd && (y1 != y2 || dx > dy); e2 += dx)
        //            {
        //                intensity = (int)Math.Max(0, 255 * (Math.Abs(e2) / ed - wd + 1));
        //                weight = intensity ^ WEIGHT_COMPLEMENT_MASK;
        //                weighted_color =
        //               (((source_alpha * weight) >> 8) << 24) |
        //               (((source_red * weight) >> 8) << 16) |
        //               (((source_green * weight) >> 8) << 8) |
        //               ((source_blue * weight) >> 8);
        //                this[x0, y2 += sy] = weighted_color;
        //            }
        //            if (x0 == x1) break;
        //            e2 = err; err -= dy; x0 += sx;
        //        }
        //        if (2 * e2 <= dy)
        //        {                                            /* y step */
        //            for (e2 = dx - e2; e2 < ed * wd && (x1 != x2 || dx < dy); e2 += dy)
        //            {
        //                intensity = (int)Math.Max(0, 255 * (Math.Abs(e2) / ed - wd + 1));
        //                weight = intensity ^ WEIGHT_COMPLEMENT_MASK;
        //                weighted_color =
        //               (((source_alpha * weight) >> 8) << 24) |
        //               (((source_red * weight) >> 8) << 16) |
        //               (((source_green * weight) >> 8) << 8) |
        //               ((source_blue * weight) >> 8);
        //                this[x2 += sx, y0] = weighted_color;
        //            }
        //            if (y0 == y1) break;
        //            err += dx; y0 += sy;
        //        }
        //    }
        //}

    }
}
