using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Graphics
{
    public class ColorChannel //NOTE: CORE UNSUPORTED, : ICloneable
    {
        double value;
        public double Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        public double DisplayValue
        {
            get { return ChannelDefinition.ValueToDisplayValueFunc(Value); }
        }

        ColorChannelDefinition channelDefinition;
        public ColorChannelDefinition ChannelDefinition
        {
            get { return channelDefinition; }
        }

        public ColorChannel(ColorChannelDefinition channelDefinition)
            : this(channelDefinition, 0.0)
        { }

        public ColorChannel(ColorChannelDefinition channelDefinition, double value)
        {
            this.channelDefinition = channelDefinition;
            this.value = value;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public static implicit operator double(ColorChannel colorChannel)
        {
            return colorChannel.Value;
        }
    }
}
