using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.PropertySystem
{
    public interface ICollectionExtendedProperty : IExtendedProperty
    {
        CollectionInheritanceMode InheritanceMode { get; set; }
    }
}
