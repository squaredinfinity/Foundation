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
        const ushort INTENSITY_BITS = 8;
        const short NUM_LEVELS = 1 << INTENSITY_BITS; // 256
        // mask used to compute 1-value by doing value XOR mask    
        const ushort WEIGHT_COMPLEMENT_MASK = NUM_LEVELS - 1; // 255    
        const ushort INTENSITY_SHIFT = (ushort)(16 - INTENSITY_BITS); // 8

        public void DrawLineWu(Rectangle bounds, int x1, int y1, int x2, int y2, int color)
        {
            var x1_d = (double)x1;
            var y1_d = (double)y1;
            var x2_d = (double)x2;
            var y2_d = (double)y2;

            if (!TryCohenSutherlandClip(bounds, ref x1_d, ref y1_d, ref x2_d, ref y2_d))
                return;

            DrawLineWu(x1, y1, x2, y2, color, 1);
        }

        public void DrawLineWu(Rectangle bounds, int x1, int y1, int x2, int y2, int color, int thickness)
        {
            var x1_d = (double)x1;
            var y1_d = (double)y1;
            var x2_d = (double)x2;
            var y2_d = (double)y2;

            if (!TryCohenSutherlandClip(bounds, ref x1_d, ref y1_d, ref x2_d, ref y2_d))
                return;

            DrawLineWu((int)x1_d, (int)y1_d, (int)x2_d, (int)y2_d, color, thickness);
        }

        public void DrawLineWu3(int x1, int y1, int x2, int y2, int color, int thickness, BlendMode blendMode)
        {
            if (thickness <= 0)
                throw new ArgumentOutOfRangeException(nameof(thickness));

            if (thickness == 1)
                thickness = 2;

            // index shift determines where the line should be drawn from based on its thickness (e.g. line with thickness 1 - no shift, thickness 2, -1px shift, thickness 3, -2px shift)        
            var index_shift = ((thickness - 1) / 2) * -1;
            var index_shift_terminator = index_shift + thickness - 1;
            ushort error_step, error_accumulator; short delta_x, delta_y, x_step; int tmp;

            // rearrange start and end points so that algorithm runs from top to bottom 
            // (in y axis, 0 is at the top, with y values increasing to the bottom)        
            if (y1 > y2)
            {
                tmp = y1;
                y1 = y2;
                y2 = tmp;

                tmp = x1;
                x1 = x2;
                x2 = tmp;
            }

            // extract components of the line colour (source)
            var sa = ((color >> 24) & 0xff);
            var sr = ((color >> 16) & 0xff);
            var sg = ((color >> 8) & 0xff);
            var sb = ((color) & 0xff);
            var isa = 255 - sa;

            // half-intensity
            var sa2 = (sa * 127) >> 8;
            var sr2 = (sr * 127) >> 8;
            var sg2 = (sg * 127) >> 8;
            var sb2 = (sb * 127) >> 8;
            var isa2 = 255 - sa2;

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

            delta_y = (short)(y2 - y1);

            // horizontal, vertical and diagonal lines are special        
            // they pass through centers of every pixel

            #region Horizontal Line

            if (delta_y == 0)
            {
                while (delta_x-- >= 0)
                {
                    SetPixelSafe(x1, y1 + index_shift, sa2, sr2, sg2, sb2, isa2, blendMode);

                    for (var y_shift = index_shift + 1; y_shift < index_shift_terminator; y_shift++)
                    {
                        SetPixelSafe(x1, y1 + y_shift, sa, sr, sg, sb, isa, blendMode);
                    }

                    SetPixelSafe(x1, y1 + index_shift_terminator, sa2, sr2, sg2, sb2, isa2, blendMode);

                    x1 += x_step;
                }

                return;
            }

            #endregion


            #region Vertical Line

            if (delta_x == 0)
            {
                while (delta_y-- >= 0)
                {
                    SetPixelSafe(x1 + index_shift, y1, sa2, sr2, sg2, sb2, isa2, blendMode);

                    for (var x_shift = index_shift + 1; x_shift < index_shift_terminator; x_shift++)
                    {
                        SetPixelSafe(x1 + x_shift, y1, sa, sr, sg, sb, isa, blendMode);
                    }

                    SetPixelSafe(x1 + index_shift_terminator, y1, sa2, sr2, sg2, sb2, isa2, blendMode);

                    y1++;
                }

                return;
            }

            #endregion


            #region Diagonal Line

            if (delta_x == delta_y)
            {
                while (delta_x-- >= 0)
                {
                    SetPixelSafe(x1 + index_shift, y1, sa2, sr2, sg2, sb2, isa2, blendMode);

                    for (var x_shift = index_shift + 1; x_shift < index_shift_terminator; x_shift++)
                    {
                        SetPixelSafe(x1 + x_shift, y1, sa, sr, sg, sb, isa, blendMode);
                    }

                    SetPixelSafe(x1 + index_shift_terminator, y1, sa2, sr2, sg2, sb2, isa2, blendMode);

                    x1 += x_step;
                    y1++;
                }

                return;
            }

            #endregion

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

                while (delta_y-- >= 0)
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
                    else
                    {
                        //pc.SetPixelSafe(x1, y1, pc.GetColor(Colors.Yellow));     
                        // invert intensity ?      
                    }

                    // calculate intensity using accumulated error  
                    var intensity = (ushort)(error_accumulator >> INTENSITY_SHIFT);

                    if (intensity > 1)
                        SetPixelSafe(x1 + index_shift_terminator, y1, sa, sr, sg, sb, isa, intensity, BlendMode.Alpha);

                    for (var x_shift = index_shift + 1; x_shift < index_shift_terminator; x_shift++)
                    {
                        if (blendMode == BlendMode.Copy)
                        {
                            SetPixelSafe(x1 + x_shift, y1, color);
                        }
                        else if (blendMode == BlendMode.Alpha)
                        {
                            SetPixelSafe(x1 + x_shift, y1, sa, sr, sg, sb, isa, blendMode);
                        }
                    }

                    intensity = (ushort)(intensity ^ WEIGHT_COMPLEMENT_MASK);

                    if (intensity > 1)
                    {
                        SetPixelSafe(x1 + index_shift, y1, sa, sr, sg, sb, isa, intensity, blendMode);
                    }

                    // move y direction        
                    y1++;
                }
            }
            else // it is X-major line  
            {
                // It's an X-major line; calculate 16-bit fixed-point fractional part of a   
                // pixel that Y advances each time X advances 1 pixel, truncating the    
                // result to avoid overrunning the endpoint along the X axis 
                error_step = (ushort)(((ulong)delta_y << 16) / (ulong)delta_x); // calculate slope * 65536

                // draw pixels between first and last           
                while (delta_x-- >= 0)
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

                    // two pixels will be set     
                    // with colors components adjusted by intensity settings   
                    // the sum of intensity of two bordering pixels equals the intensity of the line's pixel
                    // calculate intensity using accumulated error       
                    var intensity = (ushort)(error_accumulator >> INTENSITY_SHIFT);

                    if (intensity > 1)
                        SetPixelSafe(x1, y1 + index_shift_terminator, sa, sr, sg, sb, isa, intensity, blendMode);

                    for (var y_shift = index_shift + 1; y_shift < index_shift_terminator; y_shift++)
                    {
                        SetPixelSafe(x1, y1 + y_shift, sa, sr, sg, sb, isa, blendMode);
                    }

                    intensity = (ushort)(intensity ^ WEIGHT_COMPLEMENT_MASK);

                    //pc.SetPixelSafe(x1, y1 + index_shift, debug_color);

                    if (intensity > 1)
                    {
                        SetPixelSafe(x1, y1 + index_shift, sa, sr, sg, sb, isa, intensity, blendMode);
                    }
                    // move x direction            
                    x1 += x_step;
                }
            }
        }
    }
}
