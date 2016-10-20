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
using SquaredInfinity.Foundation.Graphics.Drawing;
using SquaredInfinity.Foundation.Maths.Space2D;
using SquaredInfinity.Foundation.Media.Drawing;

namespace SquaredInfinity.Foundation.Media.Drawing
{
    [TestClass]
    public class PixelArrayCanvas__Blit //: PixelCanvasTests
    {
        [TestMethod]
        public void Blit()
        {
            int count = 100;

            var sw = Stopwatch.StartNew();

            Render(count, 100);

            Trace.WriteLine("100x100: " + sw.GetElapsedAndRestart().TotalMilliseconds);

            Render(count, 1000);

            Trace.WriteLine("1kx1k: " + sw.GetElapsedAndRestart().TotalMilliseconds);

            Render(count, 2000);

            Trace.WriteLine("2kx2k: " + sw.GetElapsedAndRestart().TotalMilliseconds);
        }

        void Render(int points, int size)
        {
            var sw = Stopwatch.StartNew();

            var r = new Random();
            var pen = new Pen(Brushes.DeepPink, 2);
            pen.Freeze();

            Trace.WriteLine("after freeze: " + sw.GetElapsedAndRestart().TotalMilliseconds);

            var d = new DrawingGroup();
            using (var cx = d.Open())
            {
                for (int i = 0; i < points; i++)
                {
                    cx.DrawLine(pen, new Point(r.Next(0, 50), r.Next(0, 50)), new Point(r.Next(0, 50), r.Next(0, 50)));
                }
            }
            d.Freeze();

            var dv = new DrawingVisual();
            using (var cx = dv.RenderOpen())
            {
                cx.DrawDrawing(d);

                //for (int i = 0; i < points; i++)
                //{
                //    cx.DrawLine(pen, new Point(r.Next(0, 50), r.Next(0, 50)), new Point(r.Next(0, 50), r.Next(0, 50)));
                //}
            }

            Trace.WriteLine("after draw: " + sw.GetElapsedAndRestart().TotalMilliseconds);

            var trb = new RenderTargetBitmap(size, size, 96, 96, PixelFormats.Pbgra32);
            trb.Render(dv);

            Trace.WriteLine("after render: " + sw.GetElapsedAndRestart().TotalMilliseconds);

            var x = trb.GetPixels();

            Trace.WriteLine("after get pixels: " + sw.GetElapsedAndRestart().TotalMilliseconds);
                //trb.Save($@"C:\temp\shit\{size}.png");
        }

    [TestMethod]
        public void Bli23t()
        {
            //var pc_1 = new UnsafePixelCanvas(10, 10);
            //var pc_2 = new UnsafePixelCanvas(10, 10);

            //var sw = Stopwatch.StartNew();

            //for (int i = 0; i < 10000; i++)
            //{

                
            //    pc_1.DrawLineDDA(1, 1, 8, 1, System.Windows.Media.Colors.Black.ChangeAlpha(127));

                
            //    pc_2.DrawLineDDA(1, 1, 1, 8, System.Windows.Media.Colors.Black.ChangeAlpha(127));

            //    pc_1.Save(@"c:\temp\pc_1.bmp");
            //    pc_2.Save(@"c:\temp\pc_2.bmp");

            //    pc_1.Blit(new Rectangle(0, 0, 10, 10), pc_2, new Rectangle(0 , 0, 10, 10), BlendMode.Alpha);
            //}

            //sw.Stop();
            //Trace.WriteLine("elapsed " + sw.Elapsed.TotalMilliseconds);

            //pc_1.Save(@"c:\temp\pc_blit.bmp");
        }
    }


    [TestClass]
    public class PixelArrayCanvas__Blit2
    {
   //     [TestMethod]
//        public void Blit()
//        {
            

//            var color = (Color)ColorConverter.ConvertFromString("#007ACC");
//            var brush = new SolidColorBrush(color);

//            var pc = new PixelArrayCanvas(100, 100);

//            pc.Clear(0);

//            var line_geom = new StreamGeometry();

//            using (var ctx = line_geom.Open())
//            {
//                ctx.BeginFigure(new Point(10, 10), isFilled: false, isClosed: false);

//                // horizontal
//                ctx.LineTo(new Point(20, 25), isStroked: true, isSmoothJoin: true);
//            }

//            pc.DrawGeometry(0, 0, line_geom, Brushes.Transparent, new Pen(brush, 2));

//            pc.Save(@"c:\temp\line.bmp");


//            var marker_size = 4;
//            var MarkerHalfSize = marker_size / 2;
//            var marker_pc = new PixelArrayCanvas(marker_size, marker_size);
//            var marker_geom = new EllipseGeometry(new Point(MarkerHalfSize, MarkerHalfSize), MarkerHalfSize, MarkerHalfSize);
                                   
//            marker_pc.DrawGeometry(0, 0, marker_geom, brush, new Pen(Brushes.Transparent, 0));

//            marker_pc.Save(@"c:\temp\marker.bmp");

            
//            var first = new PixelArrayCanvas(1, 1);
//            first[0] = pc[11, 10];

//            var second = new PixelArrayCanvas(1, 1);
//            second[0] = marker_pc[3, 2];

//            var blitted = PixelCanvas.Blit(first, second, BlendMode.Alpha);


//            var result = new PixelArrayCanvas(3, 1);
//            result[0] = first[0];
//            result[1] = second[0];
//            result[2] = blitted[0];

//            result.Save(@"c:\temp\result.bmp");

//#if DEBUG

//            pc.DEBUG__Blit(
//                    new Rect(
//                        10 - MarkerHalfSize,
//                        10 - MarkerHalfSize,
//                        marker_pc.Width,
//                        marker_pc.Height),
//                        marker_pc,
//                        new Rect(0, 0, marker_pc.Width, marker_pc.Height),
//                        255, 255, 255, 255, BlendMode.Alpha);

//            pc.Save(@"c:\temp\both.bmp");

//#endif


//            first[0] = pc[11, 10];
//            first.Save(@"c:\temp\lol.bmp");

//            return;

//            //var pc_1 = new PixelArrayCanvas(10, 10);
//            //pc_1.DrawLineDDA(1, 1, 7, 8, System.Windows.Media.Colors.Red);
            
//            var wu = new PixelArrayCanvas(100, 100);
            
//            var sw = Stopwatch.StartNew();

//        //    for (int i = 0; i < 1000; i++)
//            {
//                wu = new PixelArrayCanvas(100, 100);
//                // horizontal
//                wu.DrawLineWu(0, 10, 10, 10, color);
//                // vertical
//                wu.DrawLineWu(10, 10, 10, 20, color);
//                // diagonal
//                wu.DrawLineWu(10, 20, 20, 30, color);

//                // X-major
//                wu.DrawLineWu(20, 30, 40, 40, color);

//                // Y-major
//                wu.DrawLineWu(40, 40, 50, 60, color);
//            }

//            sw.Stop();

//            Trace.WriteLine(sw.ElapsedMilliseconds);

//            wu.Save(@"c:\temp\combined.bmp");

//            sw.Start();


//         //   for (int i = 0; i < 1000; i++)
//            {
//                wu = new PixelArrayCanvas(100, 100);
//                // horizontal
//                wu.DrawLine(0, 10, 10, 10, color, 2);
//                // vertical
//                wu.DrawLine(10, 10, 10, 20, color, 2);
//                // diagonal
//                wu.DrawLine(10, 20, 20, 30, color, 2);

//                // X-major
//                wu.DrawLine(20, 30, 40, 40, color, 2);

//                // Y-major
//                wu.DrawLine(40, 40, 50, 60, color, 2);
//            }

//            sw.Stop();

//            wu.Save(@"c:\temp\combined_wpf.bmp");


//            Trace.WriteLine(sw.ElapsedMilliseconds);



//            sw.Start();


//            for (int i = 0; i < 1000; i++)
//            {
//                wu = new PixelArrayCanvas(100, 100);

//                Trace.WriteLine("1: " + sw.GetElapsedAndRestart().TotalMilliseconds);

//                var figure = new PathFigure();


//                var geom = new StreamGeometry();

//                Trace.WriteLine("2: " + sw.GetElapsedAndRestart().TotalMilliseconds);

//                using (var ctx = geom.Open())
//                {
//                    ctx.BeginFigure(new Point(0, 10), isFilled: false, isClosed: false);

//                    // horizontal
//                    ctx.LineTo(new Point(10, 10), isStroked: true, isSmoothJoin: true);
//                    //wu.DrawLineWPF(0, 10, 10, 10, color, 2);
//                    // vertical
//                    ctx.LineTo(new Point(10, 20), isStroked: true, isSmoothJoin: true);
//                    //wu.DrawLineWPF(10, 10, 10, 20, color, 2);
//                    // diagonal
//                    ctx.LineTo(new Point(20, 30), isStroked: true, isSmoothJoin: true);
//                    //wu.DrawLineWPF(10, 20, 20, 30, color, 2);

//                    // X-major
//                    ctx.LineTo(new Point(40, 40), isStroked: true, isSmoothJoin: true);
//                    //wu.DrawLineWPF(20, 30, 40, 40, color, 2);

//                    // Y-major
//                    ctx.LineTo(new Point(50, 60), isStroked: true, isSmoothJoin: true);
//                    //wu.DrawLineWPF(40, 40, 50, 60, color, 2);
//                }

//                Trace.WriteLine("3: " + sw.GetElapsedAndRestart().TotalMilliseconds);

//                wu.DrawGeometry(0, 0, geom, Brushes.Transparent, new Pen(new SolidColorBrush(Colors.Red), 2));

//                Trace.WriteLine("4: " + sw.GetElapsedAndRestart().TotalMilliseconds);
//            }

//            sw.Stop();

//            Trace.WriteLine(sw.ElapsedMilliseconds);

//            wu.Save(@"c:\temp\combined_wpf_geom.bmp");

//            //var pc = new PixelArrayCanvas(50, 48);

//            ////pc.Clear(0);

//            //pc.DrawLineWu(0, 49, 47, 0, Colors.Red, 2);
//            //pc.Save(@"c:\temp\pc_3.bmp");
//        }
    }
}
