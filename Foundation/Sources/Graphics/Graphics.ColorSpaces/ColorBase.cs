using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SquaredInfinity;
using SquaredInfinity.Graphics.ColorSpaces;
using SquaredInfinity.Extensions;

namespace SquaredInfinity.Graphics.ColorSpaces
{
    /// <summary>
    /// 
    /// </summary>
    /// <design>
    /// Name is Color**BASE** to avoid conflicts with .Net Color type when importing this namespace
    /// </design>
    public abstract class ColorBase : IColor, IEquatable<IColor>
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

        public ColorBase(IColorSpace colorSpace)
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
