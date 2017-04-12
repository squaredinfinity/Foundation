using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SquaredInfinity.Graphics.Drawing;
using SquaredInfinity.Maths;
using SquaredInfinity.Extensions;
using System.Collections.Generic;

namespace Graphics.Drawing.Tests
{
    /// <summary>
    /// Those are tests of drawing quality.
    /// Drawned lines will be compared against saved templates.
    /// </summary>
    [TestClass]
    public class PixelCanvas_DrawLineWu__DrawingQuality
    {
        int POINT_MARKER = PixelCanvas.GetColor(255, 255, 0, 220);

        [TestMethod]
        public void BUG_001__ArtifactsShowing()
        {
            var points = new List<Point2D>();

            //foreach (var line in File.ReadAllLines(@"c:\temp\1\data2.txt"))
            //{
            //    var ix = line.IndexOf(',');

            //    var p = new Point2D(double.Parse(line.Substring(0, ix)), double.Parse(line.Substring(ix + 1)));
            //    points.Add(p);
            //}


            for (var thickness = 1; thickness <= 5; thickness++)
            {
                var pc = new PixelArrayCanvas(500, 500);

                var last_point = points[0];

                foreach (var point in points.Skip(1))
                {
                    pc.DrawLineWu(last_point.X, last_point.Y, point.X, point.Y, PixelCanvas.GetColor(255, 255, 0, 0), thickness, BlendMode.Alpha);

                    pc.SetPixelSafe(last_point.X, last_point.Y, POINT_MARKER);
                    pc.SetPixelSafe(point.X, point.Y, POINT_MARKER);

                    last_point = point;
                }

                pc.Save($@"E:\t\line wu\linesegments_{thickness}.png");
            }
        }

        [TestMethod]
        public void Diagonal_Horizontal_NearDiagnoal()
        {
            var segments = new[]
            {

                new { x1 = 10, y1 = 10, x2 = 90, y2 = 90, name = "diagonal_NW_SE" },
                new { x1 = 90, y1 = 90, x2 = 10, y2 = 10, name = "diagonal_SE_NW" },
                new { x1 = 10, y1 = 90, x2 = 90, y2 = 10, name = "diagonal_SW_NE" },
                new { x1 = 90, y1 = 10, x2 = 10, y2 = 90, name = "diagonal_NE_SW" },

                new { x1 = 20, y1 = 10, x2 = 90, y2 = 90, name = "ymajor_NW_SE" },
                new { x1 = 90, y1 = 90, x2 = 20, y2 = 10, name = "ymajor_SE_NW" },
                new { x1 = 20, y1 = 90, x2 = 90, y2 = 10, name = "ymajor_SW_NE" },
                new { x1 = 90, y1 = 10, x2 = 20, y2 = 90, name = "ymajor_NE_SW" },

                new { x1 = 10, y1 = 50, x2 = 90, y2 = 50, name = "horizontal_W_E" },
                new { x1 = 90, y1 = 50, x2 = 10, y2 = 50, name = "horizontal_E_W" },

                new { x1 = 50, y1 = 10, x2 = 50, y2 = 90, name = "vertical_N_S" },
                new { x1 = 50, y1 = 90, x2 = 50, y2 = 10, name = "vertical_S_N" },

                new { x1 = 10, y1 = 20, x2 = 90, y2 = 90, name = "xmajor_NW_SE" },
                new { x1 = 90, y1 = 90, x2 = 10, y2 = 20, name = "xmajor_SE_NW" },
                new { x1 = 10, y1 = 90, x2 = 90, y2 = 20, name = "xmajor_SW_NE" },
                new { x1 = 90, y1 = 20, x2 = 10, y2 = 90, name = "xmajor_NE_SW" },

                //new { x1 = , y1 = , x2 = , y2 = , name = "" },
            };


            for (var thickness = 1; thickness <= 5; thickness++)
            {   
                foreach(var segment in segments)
                {
                    var pc = new PixelArrayCanvas(100, 100);
                    pc.DrawLineWu(segment.x1, segment.y1, segment.x2, segment.y2, PixelCanvas.GetColor(255, 255, 0, 0), thickness, BlendMode.Alpha);
                    pc.SetPixelSafe(segment.x1, segment.y1, POINT_MARKER);
                    pc.SetPixelSafe(segment.x2, segment.y2, POINT_MARKER);
                    pc.Save($@"E:\t\line wu\{segment.name}_{thickness}.png");
                }
            }
        }


        [TestMethod]
        public void AlphaBlending()
        {
            // set canvas background colour to line colour
            // all tests should return blank canvas in initial colour
            // if there are any off-colour pixel then alpha blending didn't work

            // Horizontal v Vertical
            for (var thickness = 1; thickness <= 5; thickness++)
            {
                var pc = new PixelArrayCanvas(100, 100);
                pc.Clear(PixelCanvas.GetColor(255, 255, 0, 0));
                pc.DrawLineWu(10, 50, 90, 50, PixelCanvas.GetColor(255, 255, 0, 0), thickness, BlendMode.Alpha);
                pc.DrawLineWu(50, 10, 50, 90, PixelCanvas.GetColor(255, 255, 0, 0), thickness, BlendMode.Alpha);
                pc.Save($@"E:\t\line wu\alpha_hvv_{thickness}.png");
            }

            // Diagonal v Diagonal
            for (var thickness = 1; thickness <= 5; thickness++)
            {
                var pc = new PixelArrayCanvas(100, 100);
                pc.Clear(PixelCanvas.GetColor(255, 255, 0, 0));
                pc.DrawLineWu(10, 10, 90, 90, PixelCanvas.GetColor(255, 255, 0, 0), thickness, BlendMode.Alpha);
                pc.DrawLineWu(10, 90, 90, 10, PixelCanvas.GetColor(255, 255, 0, 0), thickness, BlendMode.Alpha);
                pc.Save($@"E:\t\line wu\alpha_dvd{thickness}.png");
            }

            // X Major vs Y Major
            for (var thickness = 1; thickness <= 5; thickness++)
            {
                var pc = new PixelArrayCanvas(100, 100);
                pc.Clear(PixelCanvas.GetColor(255, 255, 0, 0));
                pc.DrawLineWu(10, 10, 90, 25, PixelCanvas.GetColor(255, 255, 0, 0), thickness, BlendMode.Alpha);
                pc.DrawLineWu(25, 10, 50, 90, PixelCanvas.GetColor(255, 255, 0, 0), thickness, BlendMode.Alpha);

                pc.Save($@"E:\t\line wu\alpha_xvy_{thickness}.png");
            }
        }

        //[TestMethod]
        //public void random()
        //{
        //    var r = new Random();

        //    for (var thickness = 1; thickness <= 5; thickness++)
        //    {
        //        var from = new Point2D(r.Next(99), r.Next(99));
        //        var pc = new PixelArrayCanvas(100, 100);

        //        for (int i = 0; i < 10; i++)
        //        {
        //            var to = new Point2D(r.Next(99), r.Next(99));
        //            pc.DrawLineWu(from.X, from.Y, to.X, to.Y, PixelCanvas.GetColor(255, 255, 0, 0), thickness, BlendMode.Alpha);

        //            pc.SetPixelSafe(from.X, from.Y, POINT_MARKER);
        //            pc.SetPixelSafe(to.X, to.Y, POINT_MARKER);

        //            from = to;
        //        }

        //        pc.Save($@"E:\t\line wu\random_{thickness}.png");
        //    }
        //}

        [TestMethod]
        public void XMajor()
        {
            for (var thickness = 1; thickness <= 5; thickness++)
            {
                // go through slopes between horizontal and diagonal
                var slope_interval = Interval.CreateOpen(Slope2D.Horizontal, Slope2D.Increasing);

                var m_id = 0;

                foreach(var m in slope_interval.LinSpace(4))
                {
                    m_id++;

                    var line = new Line2D(m, 10);

                    var x1 = 10;
                    var y1 = line.GetY(x1);
                    var x2 = 90;
                    var y2 = line.GetY(x2);

                    var pc = new PixelArrayCanvas(100, 100);
                    pc.DrawLineWu(x1, (int)y1, x2, (int)y2, PixelCanvas.GetColor(255, 255, 0, 0), thickness);
                    pc.Save($@"E:\t\line wu\xmajor_{m_id}_{thickness}.png");
                }
            }
        }

        [TestMethod]
        public void YMajor()
        {
            for (var thickness = 1; thickness <= 5; thickness++)
            {
                // go through slopes between horizontal and diagonal
                var slope_interval = Interval.CreateOpen(Slope2D.Increasing, 10);

                var m_id = 0;

                foreach (var m in slope_interval.LinSpace(4))
                {
                    m_id++;

                    var line = new Line2D(m, -100);

                    var y1 = 10;
                    var x1 = line.GetX(y1);
                    var y2 = 90;
                    var x2 = line.GetX(y2);

                    var pc = new PixelArrayCanvas(100, 100);
                    pc.DrawLineWu(pc.Bounds, (int)x1, y1, (int)x2, y2, PixelCanvas.GetColor(255, 255, 0, 0), thickness);
                    pc.Save($@"E:\t\line wu\ymajor_{m_id}_{thickness}.png");
                }
            }
        }
    }
}
