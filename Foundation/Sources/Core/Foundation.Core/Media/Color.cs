using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SquaredInfinity.Foundation;
using SquaredInfinity.Foundation.Extensions;
using SquaredInfinity.Foundation.Media.ColorSpaces;

namespace SquaredInfinity.Foundation.Media
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

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        #region Equality + Hash Code

        public override int GetHashCode()
        {
            if (Channels == null)
                return 0;

            unchecked
            {
                var hash = 17;

                for(int i = 0; i < Channels.Count; i++)
                {
                    hash = hash * 23 + Channels[i].Value.GetHashCode();
                }

                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IColor);
        }

        public virtual bool Equals(IColor other)
        {
            if (other == null)
                return false;

            if (object.ReferenceEquals(this, other))
                return true;

            foreach (var channel in this.Channels)
            {
                var otherChannel = other.Channels[channel.ChannelDefinition.Name];

                if (!channel.Value.IsCloseTo(otherChannel.Value))
                    return false;
            }

            return true;
        }

        #endregion
    }
}
