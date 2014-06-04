using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Serialization.FlexiXml.TestEntities
{
    [Serializable]
    public class Root : SimpleSerializableType
    {
        IList<ICollectionItem> _collectionReadOnlyPropertyCreatedInConstructor;
        public IList<ICollectionItem> CollectionReadOnlyPropertyCreatedInConstructor { get { return _collectionReadOnlyPropertyCreatedInConstructor; } }

        public IList<ICollectionItem> Collection { get; set; }

        public CollectionItemCollection NonGenericCollection { get; set; }

        public Root()
        {
            _collectionReadOnlyPropertyCreatedInConstructor = new CollectionItemCollection();
        }
    }
}
