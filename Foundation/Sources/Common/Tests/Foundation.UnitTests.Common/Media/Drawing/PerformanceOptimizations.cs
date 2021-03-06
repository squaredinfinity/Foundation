﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using SquaredInfinity.Foundation.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Media.Drawing
{
    [TestClass]
    public class PerformanceOptimizations
    {

        // todo, calculate randb together
        //      uint rb = (colora & 0xFF00FF) + (alpha * (colorb & 0xFF00FF)) >> 8;
        //      uint g = (colora & 0x00FF00) + (alpha * (colorb & 0x00FF00)) >> 8;
        //      return (rb & 0xFF00FF) + (g & 0x00FF00);


        [TestMethod]
        public void FastAlphaBlending()
        {
            // http://en.wikipedia.org/wiki/Alpha_compositing#Alpha_blending
            // using premultiplied values
            // out_a = a1 + a2 * (1 - a1)
            // out_rgb = r1 + r2 * (1 - a1)

            var c = new PixelArrayCanvas(0, 0);

            var foreground = c.GetColor(System.Windows.Media.Colors.Green.ChangeAlpha(127));
            var background = c.GetColor(System.Windows.Media.Colors.White.ChangeAlpha(127));

            // get premultiplied components
            var background_a = background >> 24 & 0xff;
            var background_r = background >> 16 & 0xff;
            var background_g = background >> 8 & 0xff;
            var background_b = background & 0xff;

            var foreground_a = foreground >> 24 & 0xff;
            var foreground_r = foreground >> 16 & 0xff;
            var foreground_g = foreground >> 8 & 0xff;
            var foreground_b = foreground & 0xff;

            var out_a = foreground_a + (1 - foreground_a) * background_a;
            var out_r = foreground_r + (background_r) * (1 - foreground_a);// / out_a;
            var out_g = foreground_g + (background_g) * (1 - foreground_a);// / out_a;
            var out_b = foreground_b + (background_b) * (1 - foreground_a);// / out_a;

            var pc = new PixelArrayCanvas(10, 10);
            pc.DrawLineDDA(1, 1, 1, 8, foreground);
            pc.DrawLineDDA(2, 1, 2, 8, background);
            
            pc.DrawLineDDA(4, 1, 4, 2, pc.GetColor((int)(out_a ),(int)(out_r ), (int)(out_g ),(int)(out_b )));

            var pc3 = new PixelArrayCanvas(10,10);
            pc3.DrawLineDDA(4,3,4,5, foreground);
            pc.Blit(pc3, BlendMode.Alpha);


            out_a = foreground_a + (1 - foreground_a) * background_a;
            out_r = foreground_r + (background_r) * (1 - foreground_a) / out_a;
            out_g = foreground_g + (background_g) * (1 - foreground_a) / out_a;
            out_b = foreground_b + (background_b) * (1 - foreground_a) / out_a;

            pc.DrawLineDDA(4, 6, 4, 8, pc.GetColor((int)(out_a ), (int)(out_r ), (int)(out_g ), (int)(out_b )));
            
            pc.Save(@"c:\temp\blend_xxx.bmp");



        }

        [TestMethod]
        public void FastAlphaMultiplication()
        {
            //  Pre-Multiplied RGB requires RGB channels to be pre-multiplied by Alhpa values
            //  Normally this happens by caluclating 
            //  C' = C * A/255
            //      where   C' - pre-multiplied value
            //              C  - an original channel value
            //              A  - Alpha channel value

            // operations on fractions, in particualr the division operation, are very expensive


            //

            for(int a = 0; a <= 255; a++)
            {
                for(int c = 0; c <= 255; c++)
                {
                    var pc = Math.Round(c * ((double)a / 255), MidpointRounding.AwayFromZero);

                    // c' = c * (a/255)
                    // c' = c * a * 1/255
                    // c' = ac * 1/255

                    // to avoid 1/255 division we can make use of bit pattern of fractions in a form of 1/(2^n - 1)
                    //
                    //  (1/3)   = 0.010101010101..
                    //  (1/7)   = 0.001001001001..
                    //  (1/15)  = 0.000100010001..
                    //  ...
                    //  (1/255) = 0.0000000100000001...) = 0.0101.. in hex
                    //
                    // Here we can use a trick developed by Alvy Ray Smith, described by Jim Blinn in Dirty Pixels
                    // were to divide by 255 he multiplied by 0x101 and divided by 0x10000
                    // y = x / 255 => y = (( x << 8 ) + x ) >> 16
                    //
                    // which leaves us with
                    //
                    //  c' = ((ac << 8) + ac) >> 16)
                    // NOTE that this produces slight precision loss (+/- 1)
                    //
                    //

                    //var r = (((a*c) << 8) + (a*c)) >> 16;

                    //var i = (a * c) + 1;

                    //r = ((i << 8) + i) >> 16;

                    //if (pc != r)
                    //{
                    //    Debug.WriteLine("a:\t{0}\tc:{1}\texpected:\t{2}\tactual:\t{3}\terror:\t{4}".FormatWith(a, c, pc, r, r - pc));
                    //}
                    
                    // 

                    // uses only integers and avoids division
                    // no data loss
                    var i = ((c * a) + 128);
                    var r = (i + (i >> 8)) >> 8;
                    
//                    Assert.AreEqual(pc, r);

                    // above works well, but we need to run same code against three channels (R,G,B)
                    // so further reduction of operations is critical


                    // if two bytes are multiplied, we can get a result with quite good precision from High-Order Byte (HOB)
                    // eg: 
                    //  255 * 255/255 = 255
                    //  255 * 255 = 65025 | 1111 1110 0000 0001 <- notice 8th bit is 0 instead of 1, HOB = 254
                    //  over the whole range of possible byte values 
                    //  extreme error of -2 affects ~2% of cases
                    //  and error of -1 affects ~70% of cases
                    //
                    // we could add 1 to first multiplicand in order to reduce maximum error to 1 (from 2) and number of affected cases to ~29%

                    int ri = -1;

                    if(a == 0)
                    {
                        ri = 0;
                    }
                    else if (a == 255)
                    {
                        ri = c;
                    }
                    else
                    {
                        // rounding error affects ~70% of results
                        //ri = ((c * a) >> 8); 

                        // rounding error affects ~30% of results
                        var ai = a + 1;
                        ri = ((c * ai) >> 8);
                    }

                    if(r != ri)
                    {
                        Trace.WriteLine("a\t{0}\tc\t{1}\texpected\t{2}\tactual\t{3}\terror\t{4}".FormatWith(a, c, pc, ri, ri - r));
                    }
                }
            }

            var x = 0;
        }
    }
}
