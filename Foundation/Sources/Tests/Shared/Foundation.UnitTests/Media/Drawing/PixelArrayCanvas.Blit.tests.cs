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
using System.Diagnostics;
using System.Windows;

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


    [TestClass]
    public class PixelArrayCanvas__Blit
    {
        [TestMethod]
        public void Blit()
        {
            //var pc_1 = new PixelArrayCanvas(10, 10);
            //pc_1.DrawLineDDA(1, 1, 7, 8, System.Windows.Media.Colors.Red);

            var color = Colors.Red;
            var wu = new PixelArrayCanvas(100, 100);
            
            var sw = Stopwatch.StartNew();

        //    for (int i = 0; i < 1000; i++)
            {
                wu = new PixelArrayCanvas(100, 100);
                // horizontal
                wu.DrawLineWu(0, 10, 10, 10, color);
                // vertical
                wu.DrawLineWu(10, 10, 10, 20, color);
                // diagonal
                wu.DrawLineWu(10, 20, 20, 30, color);

                // X-major
                wu.DrawLineWu(20, 30, 40, 40, color);

                // Y-major
                wu.DrawLineWu(40, 40, 50, 60, color);
            }

            sw.Stop();

            Trace.WriteLine(sw.ElapsedMilliseconds);

            wu.Save(@"c:\temp\combined.bmp");

            sw.Start();


         //   for (int i = 0; i < 1000; i++)
            {
                wu = new PixelArrayCanvas(100, 100);
                // horizontal
                wu.DrawLine(0, 10, 10, 10, color, 2);
                // vertical
                wu.DrawLine(10, 10, 10, 20, color, 2);
                // diagonal
                wu.DrawLine(10, 20, 20, 30, color, 2);

                // X-major
                wu.DrawLine(20, 30, 40, 40, color, 2);

                // Y-major
                wu.DrawLine(40, 40, 50, 60, color, 2);
            }

            sw.Stop();

            wu.Save(@"c:\temp\combined_wpf.bmp");


            Trace.WriteLine(sw.ElapsedMilliseconds);



            sw.Start();


           // for (int i = 0; i < 1000; i++)
            {
                wu = new PixelArrayCanvas(1, 1);

                var figure = new PathFigure();


                var geom = new StreamGeometry();
                using (var ctx = geom.Open())
                {
                    ctx.BeginFigure(new Point(0, 10), isFilled: false, isClosed: false);

                    // horizontal
                    ctx.LineTo(new Point(10, 10), isStroked: true, isSmoothJoin: true);
                    //wu.DrawLineWPF(0, 10, 10, 10, color, 2);
                    // vertical
                    ctx.LineTo(new Point(10, 20), isStroked: true, isSmoothJoin: true);
                    //wu.DrawLineWPF(10, 10, 10, 20, color, 2);
                    // diagonal
                    ctx.LineTo(new Point(20, 30), isStroked: true, isSmoothJoin: true);
                    //wu.DrawLineWPF(10, 20, 20, 30, color, 2);

                    // X-major
                    ctx.LineTo(new Point(40, 40), isStroked: true, isSmoothJoin: true);
                    //wu.DrawLineWPF(20, 30, 40, 40, color, 2);

                    // Y-major
                    ctx.LineTo(new Point(50, 60), isStroked: true, isSmoothJoin: true);
                    //wu.DrawLineWPF(40, 40, 50, 60, color, 2);
                }                

                wu.DrawGeometry(50, 50, geom, new Pen(new SolidColorBrush(Colors.Red), 2));
            }

            sw.Stop();

            Trace.WriteLine(sw.ElapsedMilliseconds);

            wu.Save(@"c:\temp\combined_wpf_geom.bmp");

            //var pc = new PixelArrayCanvas(50, 48);

            ////pc.Clear(0);

            //pc.DrawLineWu(0, 49, 47, 0, Colors.Red, 2);
            //pc.Save(@"c:\temp\pc_3.bmp");
        }
    }
}
