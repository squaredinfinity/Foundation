using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Serialization.FlexiXmlSerialization.Entities
{
    public enum ParticipationMode
    {
        /// <summary>
        /// Types or Members will participate in serialization by default (unless explicitely excluded).
        /// </summary>
        OptOut,

        /// <summary>
        /// Types or Members will NOT participate in serialization by default (unless explicitely included).
        /// </summary>
        OptIn
    }
}
