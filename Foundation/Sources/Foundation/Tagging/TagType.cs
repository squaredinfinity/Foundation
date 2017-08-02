using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Tagging
{
    public enum TagType
    {
        /// <summary>
        /// Tag stores single key-value pair
        /// </summary>
        SingleValue,
        /// <summary>
        /// Tag stores multiple values per key
        /// </summary>
        MultiValue,
        /// <summary>
        /// Tag does not store values, just a key
        /// </summary>
        Marker
    }
}
