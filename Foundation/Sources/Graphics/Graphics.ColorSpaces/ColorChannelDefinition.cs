using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SquaredInfinity.Graphics.ColorSpaces;

namespace SquaredInfinity.Graphics
{
    public class ColorChannelDefinition
    {
        readonly string name;
        public string Name 
        {
            get { return name; } 
        }

        readonly string shortName;
        public string ShortName
        {
            get { return shortName; } 
        }

        readonly Func<double, double> valueToDisplayValueFunc;
        internal Func<double, double> ValueToDisplayValueFunc
        {
            get { return valueToDisplayValueFunc; }
        }

        readonly IColorSpace colorSpace;
        public IColorSpace ColorSpace
        {
            get { return this.colorSpace; } 
        }

        internal ColorChannelDefinition(IColorSpace colorSpace, string channelName, string shortChannelName)
            : this(colorSpace, channelName, shortChannelName, (v) => v)
        { }

        internal ColorChannelDefinition(IColorSpace colorSpace, string channelName, string shortChannelName, Func<double, double> valueToDisplayValueFunc)
        {
            this.colorSpace = colorSpace;
            this.name = channelName;
            this.shortName = shortChannelName;
            this.valueToDisplayValueFunc = valueToDisplayValueFunc;
        }
    }
}
