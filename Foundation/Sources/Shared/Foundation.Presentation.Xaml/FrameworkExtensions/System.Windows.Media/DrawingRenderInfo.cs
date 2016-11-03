using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace SquaredInfinity.Foundation.Extensions
{
    public class DrawingRenderInfo
    {
        public Drawing Drawing { get; set; }
        public Transform Transform { get; set; }
        public double? Opacity { get; set; }
        public Geometry Clip { get; set; }
    }
}
