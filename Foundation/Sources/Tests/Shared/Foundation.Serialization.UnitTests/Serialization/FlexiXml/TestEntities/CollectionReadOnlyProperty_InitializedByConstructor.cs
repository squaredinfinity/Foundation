using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Serialization.FlexiXml.TestEntities
{
    public class TypeWithCollectionReadOnlyProperty
    {
        List<int> _items = new List<int>();

        // This property is a collection and it is read-only (but the collection itself isn't)
        public List<int> Items { get { return _items; } }
    }
}
