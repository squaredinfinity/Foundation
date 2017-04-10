using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Tagging
{
    public enum MissingTagBehavior
    {
        /// <summary>
        /// When a tag is missing in source collection  but exists in target collection, the target collection will keep it  an its values.
        /// </summary>
        Keep,
        /// <summary>
        /// When a tag is missing in source collection  but exists in target collection, the target collection will remove it and all its values.
        /// </summary>
        Remove
    }
}
