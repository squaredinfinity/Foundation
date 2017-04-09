using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SquaredInfinity.Graphics.Drawing;
using SquaredInfinity.Maths;
using SquaredInfinity.Extensions;

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
                    pc.DrawLineWu(segment.x1, segment.y1, segment.x2, segment.y2, PixelCanvas.GetColor(255, 255, 0, 0), thickness);
                    //pc.SetPixelSafe(segment.x1, segment.y1, POINT_MARKER);
                    //pc.SetPixelSafe(segment.x2, segment.y2, POINT_MARKER);
                    pc.Save($@"E:\t\line wu\{segment.name}_{thickness}.png");
                }
            }
        }

        //[TestMethod]
        //public void lol()
        //{
        //    for (var thickness = 1; thickness <= 5; thickness++)
        //    {
        //        foreach (var x in Interval.CreateClosed(0, 100).LinSpace(10))
        //        {
        //        }

        //        foreach (var segment in segments)
        //        {
        //            var pc = new PixelArrayCanvas(100, 100);
        //            pc.DrawLineWu(segment.x1, segment.y1, segment.x2, segment.y2, PixelCanvas.GetColor(255, 255, 0, 0), thickness);
        //            pc.SetPixelSafe(segment.x1, segment.y1, POINT_MARKER);
        //            pc.SetPixelSafe(segment.x2, segment.y2, POINT_MARKER);
        //            pc.Save($@"E:\t\line wu\{segment.name}_{thickness}.png");
        //        }
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
