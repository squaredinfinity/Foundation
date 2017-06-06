using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SquaredInfinity.Maths.DigitalSignalProcessing.PolygonSimplification;
using SquaredInfinity.Maths;
using System.Collections.Generic;
using System.Diagnostics;

namespace DigitalSignalProcessing.UnitTests
{
    [TestClass]
    public class IPolygon2dIndexesSimplification__GENERAL
    {
        IReadOnlyList<Point2D> GetDefaultPoints() => new List<Point2D>
        {
            //                              ix  distance to last
            new Point2D(0, 0),          //  [0] ---
            new Point2D(1, 1),          //  [1]   1.414
            new Point2D(4, 1),          //  [2]   3
            new Point2D(4.5, 1.5),      //  [3]   0.707
            new Point2D(10, 10),        //  [4]  10.124
            new Point2D(10, 10),        //  [5]   0
            new Point2D(100, 100),      //  [6] 127.279
            new Point2D(100.5, 100.5)   //  [7]   0.707
        };

        void PrintDistnaces(IReadOnlyList<Point2D> points)
        {
            for (int i = 0; i < points.Count - 1; i++)
                Trace.WriteLine(points[i].Distance(points[i + 1]));
        }

        [TestMethod]
        public void keeps_first_and_last_points()
        {
            var ps = new RadialDistancePolygon2DIndexesSimplification(3.0);

            var simplified = ps.SimplifyIndexes(GetDefaultPoints(), x => x);
            
            Assert.AreEqual(0, simplified.Indexes[0]);
            Assert.AreEqual(7, simplified.Indexes.Last());
        }

        [TestMethod]
        public void removes_points()
        {
            var ps = new RadialDistancePolygon2DIndexesSimplification(2.0);
            
            var simplified = ps.SimplifyIndexes(GetDefaultPoints(), x => x);

            var expected_indexes = new int[] { 0, 2, 4, 6, 7 };

            CollectionAssert.AreEqual(expected_indexes, simplified.Indexes.ToArray());
        }
    }
}
