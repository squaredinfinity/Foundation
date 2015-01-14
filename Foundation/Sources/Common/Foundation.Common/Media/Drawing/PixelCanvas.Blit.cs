using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Media.Drawing
{
    public partial class
#if UNSAFE
        UnsafePixelCanvas
#else
 PixelCanvas
#endif
    {
        public enum BlendMode
        {
            /// <summary>
            /// Alpha blendiing uses the alpha channel to combine the source and destination. 
            /// </summary>
            Alpha,

            /// <summary>
            /// Additive blending adds the colors of the source and the destination.
            /// </summary>
            Additive,

            /// <summary>
            /// Subtractive blending subtracts the source color from the destination.
            /// </summary>
            Subtractive,

            /// <summary>
            /// Uses the source color as a mask.
            /// </summary>
            Mask,

            /// <summary>
            /// Multiplies the source color with the destination color.
            /// </summary>
            Multiply,

            /// <summary>
            /// Ignores the specified Color
            /// </summary>
            ColorKeying,

            /// <summary>
            /// No blending just copies the pixels from the source.
            /// </summary>
            None,

            Debug
        }

        public void Blit(
            System.Drawing.Rectangle destination_rect,
            IPixelCanvas source,
            System.Drawing.Rectangle source_rect,
            byte alpha,
            byte red,
            byte green,
            byte blue,
            BlendMode blendMode)
        {
            if (alpha == 0)
                return;

            if (!IntersectsWith(destination_rect))
                return;

            // tinted if color is not opaque white
            var is_tinted = alpha != 255 || red != 255 || green != 255 || blue != 255;

            var source_width = source.Width;
            var source_pixels = source;
            var source_length = source.Length;

            int source_rectangle_x = source_rect.X;
            int source_rectangle_y = source_rect.Y;

            var destination_width = Width;
            var destination_height = Height;
            var destination_pixels = this;
            var destination_length = Length;

            int destination_rectangle_x = destination_rect.X;
            int destination_rectangle_y = destination_rect.Y;
            int destination_rect_width = destination_rect.Width;
            int destination_rect_height = destination_rect.Height;

            int sourceIdx = -1;
            int idx;
            double ii;
            double jj;

            int source_pixel = 0;
            int source_alpha = 0;
            int source_red = 0;
            int source_green = 0;
            int source_blue = 0;

            int destination_alpha;
            int destination_red;
            int destination_green;
            int destination_blue;
            
            var source_rectangle_width = source_rect.Width;

            var sdx = source_rect.Width / destination_rect.Width;
            var sdy = source_rect.Height / destination_rect.Height;

            int lastii = -1;
            int lastjj = -1;

            jj = source_rectangle_y;


            int current_x;
            int current_y = destination_rectangle_y;

            // each row in destination rectangle
            for (int destination_rect_row = 0; destination_rect_row < destination_rect_height; destination_rect_row++)
            {
                // check if current y is within destination rectangle bounds (todo: >= 0? should it be destination_rect_y ?)
                if (current_y >= 0 && current_y < destination_height)
                {
                    ii = source_rectangle_x;
                    idx = destination_rectangle_x + current_y * destination_width;

                    // start from left most column in destination rectangle
                    current_x = destination_rectangle_x;

                    // each column in destination rectangle
                    for (int destination_rect_col = 0; destination_rect_col < destination_rect_width; destination_rect_col++)
                    {
                        // todo: >= 0?
                        if (current_x >= 0 && current_x < destination_width)
                        {
                            if ((int)ii != lastii || (int)jj != lastjj)
                            {
                                sourceIdx = (int)ii + (int)jj * source_width;

                                if (sourceIdx >= 0 && sourceIdx < source_length)
                                {
                                    // retrieve current source pixel and it's channels values
                                    source_pixel = source_pixels[sourceIdx];

                                    source_alpha = ((source_pixel >> 24) & 0xff);
                                    source_red = ((source_pixel >> 16) & 0xff);
                                    source_green = ((source_pixel >> 8) & 0xff);
                                    source_blue = ((source_pixel) & 0xff);

                                    // tint the pixel if needed
                                    if (is_tinted && source_alpha != 0)
                                    {
                                        source_alpha = (((source_alpha * alpha) * 0x8081) >> 23);
                                        source_red = ((((((source_red * red) * 0x8081) >> 23) * alpha) * 0x8081) >> 23);
                                        source_green = ((((((source_green * green) * 0x8081) >> 23) * alpha) * 0x8081) >> 23);
                                        source_blue = ((((((source_blue * blue) * 0x8081) >> 23) * alpha) * 0x8081) >> 23);

                                        source_pixel = (source_alpha << 24) | (source_red << 16) | (source_green << 8) | source_blue;
                                    }
                                }
                                else
                                {
                                    source_alpha = 0;
                                }
                            }
                            if (blendMode == BlendMode.None)
                            {
                                destination_pixels[idx] = source_pixel;
                            }
                            else if (source_alpha > 0)
                            {
                                int destPixel = destination_pixels[idx];
                                destination_alpha = ((destPixel >> 24) & 0xff);
                                if ((source_alpha == 255 || destination_alpha == 0) &&
                                   blendMode != BlendMode.Additive
                                   && blendMode != BlendMode.Subtractive
                                   && blendMode != BlendMode.Multiply
                                   )
                                {
                                    destination_pixels[idx] = source_pixel;
                                }
                                else
                                {
                                    destination_red = ((destPixel >> 16) & 0xff);
                                    destination_green = ((destPixel >> 8) & 0xff);
                                    destination_blue = ((destPixel) & 0xff);
                                    if (blendMode == BlendMode.Alpha)
                                    {
                                        var isa = 255 - source_alpha;

                                        destPixel = ((destination_alpha & 0xff) << 24) |
                                                    (((((source_red << 8) + isa * destination_red) >> 8) & 0xff) << 16) |
                                                    (((((source_green << 8) + isa * destination_green) >> 8) & 0xff) << 8) |
                                                     ((((source_blue << 8) + isa * destination_blue) >> 8) & 0xff);
                                    }

                                    destination_pixels[idx] = destPixel;
                                }
                            }
                        }
                        current_x++;
                        idx++;
                        ii += sdx;
                    }
                }
                jj += sdy;
                current_y++;
            }
        }
    }
}