using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Serialization.FlexiXml.TestEntities
{
    public interface ICollectionItem
    {
        string StringProperty { get; set; }
        int IntProperty { get; set; }
    }

    public class CollectionItem : ICollectionItem
    {
        public string StringProperty { get; set; }

        public int IntProperty { get; set; }
    }

    public class CollectionItemWithReference : CollectionItem
    {
        public IList<ICollectionItem> ParentCollection { get; set; }
    }

    public class CollectionItemCollection : Collection<ICollectionItem>
    {
        public string StringProperty { get; set; }
        public int IntProperty { get; set; }
    }
}
