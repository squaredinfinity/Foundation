using Microsoft.VisualStudio.TestTools.UnitTesting;
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
                    // NOTE that this produces slighr precision loss (+/- 1)
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
