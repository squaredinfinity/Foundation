using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using SquaredInfinity.Foundation.Extensions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SquaredInfinity.Foundation.Media.Drawing
{
    //[TestClass]
    //public class PixelArrayCanvas__Blit : PixelCanvasTests
    //{
    //    [TestMethod]
    //    public void Blit()
    //    {
    //        var pc_1 = new PixelArrayCanvas(10, 10);
    //        pc_1.DrawLineDDA(1, 1, 8, 1, System.Windows.Media.Colors.Black.ChangeAlpha(127));

    //        var pc_2 = new PixelArrayCanvas(10, 10);
    //        pc_2.DrawLineDDA(1, 1, 1, 8, System.Windows.Media.Colors.Black.ChangeAlpha(127));

    //        pc_1.Save(@"c:\temp\pc_1.bmp");
    //        pc_2.Save(@"c:\temp\pc_2.bmp");
            
    //        pc_1.Blit(new System.Drawing.Rectangle(0, 0, 10, 10), pc_2, new System.Drawing.Rectangle(0,0,10,10), BlendMode.Alpha);

    //        pc_1.Save(@"c:\temp\pc_blit.bmp");
    //    }
    //}
}
