using SquaredInfinity.Graphics.ColorSpaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Graphics
{
    public interface IColor //NOTE: CORE UNSUPORTED, : ICloneable
    {
        /// <summary>
        /// Returns the color space for which this color is defined.
        /// </summary>
        IColorSpace ColorSpace { get; }

        ColorChannelCollection Channels { get; }


        ///// <summary>
        ///// Returns the signal value for specified channel.
        ///// </summary>
        ///// <param name="channel"></param>
        ///// <returns></returns>
        //double GetSignalValue(ColorChannel channel);

        ///// <summary>
        ///// Sets the signal value for specified channel.
        ///// </summary>
        ///// <param name="channel"></param>
        ///// <param name="newValue"></param>
        //void SetSignalValue(ColorChannel channel, double newValue);

        ///// <summary>
        ///// Returns the digital representation of signal value (e.g. 8-bit value in range of 0-255)
        ///// </summary>
        ///// <param name="channel"></param>
        ///// <param name="bits"></param>
        ///// <returns></returns>
        //double GetDigitalValue(ColorChannel channel, int bits);

        ///// <summary>
        ///// Sets the digital representations of signal value (e.g. 8-bit value in range of 0-255)
        ///// </summary>
        ///// <param name="channel"></param>
        ///// <param name="newValue"></param>
        ///// <param name="bits"></param>
        //void SetDigitalValue(ColorChannel channel, double newValue, int bits);
    }
}
