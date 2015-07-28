using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.PropertySystem
{
    public interface IExtendedCollectionPropertyDefinition<TItem> : IExtendedPropertyDefinition
    {
        Func<IList<TItem>> GetDefaultValue { get; }
        IList<TItem> GetActualValue(IExtendedPropertyContainer container);
    }
}
