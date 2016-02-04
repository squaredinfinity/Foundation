using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.Foundation.Serialization.FlexiXml
{
    public enum DeserializationOutcome
    {
        /// <summary>
        /// cannot deserialize because it is an internal object (e.g. type lookup table)
        /// </summary>
        IgnoreInternal,
        /// <summary>
        /// Failed to deserialize
        /// </summary>
        Error,
        /// <summary>
        /// Deserialization succeded
        /// </summary>
        Success
    }
}
