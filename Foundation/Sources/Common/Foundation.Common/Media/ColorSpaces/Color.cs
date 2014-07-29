using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SquaredInfinity.Foundation;
using SquaredInfinity.Foundation.Extensions;

namespace SquaredInfinity.Foundation.Media.ColorSpaces
{
    public abstract class Color : IColor, IEquatable<IColor>
    {
        public IColorSpace ColorSpace
        {
            get;
            private set;
        }

        public ColorChannelCollection Channels
        {
            get;
            protected set;
        }

        public Color(IColorSpace colorSpace)
        {
            this.ColorSpace = colorSpace;
        }

        public virtual object Clone()
        {
            return this.MemberwiseClone();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IColor);
        }

        public bool Equals(IColor other)
        {
            if (this.GetType() != other.GetType())
                return false;

            foreach (var channel in this.Channels)
            {
                var otherChannel = other.Channels[channel.ChannelDefinition.Name];

                if (!channel.Value.IsCloseTo(otherChannel.Value))
                    return false;
            }

            return true;
        }
    }
}
