using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SquaredInfinity.Foundation.Extensions;

namespace SquaredInfinity.Foundation.PropertySystem
{
    public class CollectionExtendedProperty<TItem> : ExtendedProperty<Collection<TItem>>
    {
        CollectionInheritanceMode _inheritanceMode = CollectionInheritanceMode.Merge;
        public CollectionInheritanceMode InheritanceMode
        {
            get { return _inheritanceMode; }
            set { _inheritanceMode = value; }
        }

        public CollectionExtendedProperty(IExtendedPropertyCollection owner, string uniqueName, Func<Collection<TItem>> getDefaultValue)
            : this(owner, uniqueName, getDefaultValue, CollectionInheritanceMode.Merge)
        { }

        public CollectionExtendedProperty(IExtendedPropertyCollection owner, string uniqueName, Func<Collection<TItem>> getDefaultValue, CollectionInheritanceMode inheritanceMode)
            : base(owner, uniqueName, getDefaultValue)
        {
            this.InheritanceMode = inheritanceMode;
        }

        protected override Collection<TItem> GetActualValue()
        {
            if (IsValueSet)
            {
                if (InheritanceMode == CollectionInheritanceMode.Replace)
                    return Value;
                else
                {
                    object inheritedValue = null;
                    if (Owner.TryGetInheritedPropertyValue(UniqueName, out inheritedValue))
                    {
                        if (Value == null)
                        {
                            if (inheritedValue == null)
                            {
                                return GetDefaultValue();
                            }
                            else
                            {
                                return Value;
                            }
                        }
                        else
                        {
                            if (inheritedValue == null)
                            {
                                return Value;
                            }
                            else
                            {
                                var result = new Collection<TItem>();

                                result.AddRange((IEnumerable<TItem>)inheritedValue);

                                result.AddRange((IEnumerable<TItem>)Value);

                                return result;
                            }
                        }
                    }
                    else
                    {
                        return Value;
                    }
                }
            }
            else
            {
                object inheritedValue = null;
                if (Owner.TryGetInheritedPropertyValue(UniqueName, out inheritedValue))
                {
                    return (Collection<TItem>)inheritedValue;
                }
                else
                {
                    return GetDefaultValue();
                }
            }
        }
    }
}
