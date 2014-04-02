using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Foundation.Types.Mapping
{
    public class MappingOptions
    {
        public static readonly MappingOptions Default;

        static MappingOptions()
        {
            Default = new MappingOptions
            {
                IgnoreNulls = false
            };
        }

        /// <summary>
        /// When *true*, source members with NULL value will not be mapped (original value of the target will be preserved)
        /// </summary>
        public bool IgnoreNulls { get; set; }
    }
}
