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
using System.Threading;

namespace SquaredInfinity.Foundation.Media.Drawing
{
    [TestClass]
    public class PixelArrayCanvas__Blit //: PixelCanvasTests
    {
        [TestMethod]
        public void Blit()
        {
            var rd = new Random();

            var rc = new RawDrawCommand((pc, ct) =>
            {
                pc.DrawRectangle(10, 10, 20, 20, pc.GetColor(Colors.Red));
            });

            var wpfc = new WpfDrawingContextDrawCommand((pc, cx, ct) =>
            {
                var c = pc.GetColor(Colors.Turquoise);

                var pen = new Pen(Brushes.Turquoise, 2.0);
                pen.Freeze();

                for (int i = 0; i < 100; i++)
                {
                    var p1 =
                    new Point(
                            rd.Next(1000, 1300),
                            rd.Next(1000, 1300));

                    var p2 =
                    new Point(
                            rd.Next(1000, 1300),
                            rd.Next(1000, 1300));

                    if (p1.ToPoint2D().Distance(p2.ToPoint2D()).IsLessThan(1))
                        continue;

                    pc.DrawLineWu((int)p1.X, (int)p1.Y, (int)p2.X, (int)p2.Y, c);
                    //_cx.DrawLine(pen, p1, p2);
                }
            });

            var wpfc2 = new WpfDrawingContextDrawCommand((pc, cx, ct) =>
            {
                for (int i = 0; i < 1000; i++)
                {
                    var pen = new Pen(Brushes.Moccasin, 2.0);
                    pen.Freeze();

                    cx.DrawLine(pen,
                        new Point(
                            rd.Next(750, 900),
                            rd.Next(750, 900)),
                        new Point(
                            rd.Next(750, 900),
                            rd.Next(750, 900)));
                }
            });

            var wpfc3 = new WpfDrawingContextDrawCommand((pc, cx, ct) =>
            {
                for (int i = 0; i < 1000; i++)
                {
                    var pen = new Pen(Brushes.Moccasin, 2.0);
                    pen.Freeze();

                    cx.DrawLine(pen,
                        new Point(
                            rd.Next(500, 600),
                            rd.Next(500, 600)),
                        new Point(
                            rd.Next(500, 600),
                            rd.Next(500, 600)));
                }
            });

            var wpfc4 = new WpfDrawingContextDrawCommand((pc, cx, ct) =>
            {
                for (int i = 0; i < 1000; i++)
                {
                    var pen = new Pen(Brushes.Ivory, 2.0);
                    pen.Freeze();

                    cx.DrawLine(pen,
                        new Point(
                            rd.Next(0, 2000),
                            rd.Next(50, 2500)),
                        new Point(
                            rd.Next(0, 2000),
                            rd.Next(50, 2500)));
                }
            });

            var wpfc5 = new WpfDrawingContextDrawCommand((pc, cx, ct) =>
            {
                for (int i = 0; i < 1000; i++)
                {
                    var pen = new Pen(Brushes.Navy, 2.0);
                    pen.Freeze();

                    cx.DrawLine(pen,
                        new Point(
                            rd.Next(0, 2000),
                            rd.Next(50, 500)),
                        new Point(
                            rd.Next(0, 2000),
                            rd.Next(50, 500)));
                }
            });

            var wpfc6 = new WpfDrawingContextDrawCommand((pc, cx, ct) =>
            {
                for (int i = 0; i < 1000; i++)
                {
                    var pen = new Pen(Brushes.Crimson, 2.0);
                    pen.Freeze();

                    cx.DrawLine(pen,
                        new Point(
                            rd.Next(100, 200),
                            rd.Next(100, 200)),
                        new Point(
                            rd.Next(100, 200),
                            rd.Next(100, 200)));
                }
            });

            var wpfc7 = new WpfDrawingContextDrawCommand((pc, cx, ct) =>
            {
                for (int i = 0; i < 1000; i++)
                {
                    var pen = new Pen(Brushes.Coral, 2.0);
                    pen.Freeze();

                    cx.DrawLine(pen,
                        new Point(
                            rd.Next(0, 50),
                            rd.Next(0, 50)),
                        new Point(
                            rd.Next(0, 50),
                            rd.Next(0, 50)));
                }
            });

            var sw = Stopwatch.StartNew();
            var canvas = new PixelArrayCanvas(2000, 2000);
            Trace.WriteLine("CREATED " + sw.GetElapsedAndRestart().TotalMilliseconds);
            //pc.Execute(new CanvasCommand[] { rc, wpfc, wpfc2, wpfc3, wpfc4, wpfc5, wpfc6, wpfc7 });
            canvas.Execute(new CanvasCommand[] { rc, wpfc }, CancellationToken.None);
            Trace.WriteLine("EXECUTE " + sw.GetElapsedAndRestart().TotalMilliseconds);
            var bmp = canvas.ToWriteableBitmap();
            Trace.WriteLine("BMP " + sw.GetElapsedAndRestart().TotalMilliseconds);
            canvas.Save(@"c:\temp\shit\1.png");
            Trace.WriteLine("SAVE " + sw.GetElapsedAndRestart().TotalMilliseconds);


            //InvocationThrottle t = new InvocationThrottle();

            //var total_ms = 0.0;

            //for (int i = 0; i < 10000; i++)
            //{
            //    Thread.Sleep(10);
            //    var g = Guid.NewGuid();

            //    var sw = Stopwatch.StartNew();
            //    t.InvokeAsync((ct) =>
            //    {
            //        Thread.Sleep(75);
            //        Trace.WriteLine("RUN TO COMPLETION");
            //    });

            //    sw.Stop();
            //    total_ms += sw.Elapsed.TotalMilliseconds;
            //    Trace.WriteLine("WAIT: " + sw.Elapsed.Milliseconds);
            //    Trace.WriteLine("WAIT ALL: " + total_ms);
            //}

            //Thread.Sleep(10000);

            //var sw = Stopwatch.StartNew();

            ////for (int i = 0; i < 10000; i++)
            //{
            //    var pen = new PixelArrayCanvas(10, 10);
            //    pen.DrawRectangle(0, 0, 3, 3, pen.GetColor(Colors.Red));

            //    var cv = new PixelArrayCanvas(1000, 1000);
            //    cv.DrawLineWu(100, 100, 900, 900, cv.GetColor(Colors.Red));

            //    cv.Save(@"c:\temp\shit\1.png");
            //}

            //sw.Stop();

            //Trace.WriteLine(sw.Elapsed.TotalMilliseconds);

            //int count = 100;

            //var sw = Stopwatch.StartNew();

            //Render(count, 100);

            //Trace.WriteLine("100x100: " + sw.GetElapsedAndRestart().TotalMilliseconds);

            //Render(count, 1000);

            //Trace.WriteLine("1kx1k: " + sw.GetElapsedAndRestart().TotalMilliseconds);

            //Render(count, 2000);

            //Trace.WriteLine("2kx2k: " + sw.GetElapsedAndRestart().TotalMilliseconds);
        }

        void Render(int points, int size)
        {
            var sw = Stopwatch.StartNew();

            var r = new Random();
            var pen = new Pen(Brushes.DeepPink, 2);
            pen.Freeze();

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
