using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Serialization.FlexiXml.TestEntities
{
    public class TypeWithNonGenericCollectionReadOnlyProperty
    {
        NonGenericCollection _items = new NonGenericCollection();

        // This property is a collection and it is read-only (but the collection itself isn't)
        public NonGenericCollection Items { get { return _items; } }
    }
}
