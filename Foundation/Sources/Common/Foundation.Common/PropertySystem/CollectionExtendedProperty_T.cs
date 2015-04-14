using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SquaredInfinity.Foundation.Extensions;

namespace SquaredInfinity.Foundation.PropertySystem
{
    public class CollectionExtendedProperty<TItem> : ExtendedProperty<IList<TItem>>, ICollectionExtendedProperty
    {
        CollectionInheritanceMode _inheritanceMode = CollectionInheritanceMode.Merge;
        public CollectionInheritanceMode InheritanceMode
        {
            get { return _inheritanceMode; }
            set { _inheritanceMode = value; }
        }

        public CollectionExtendedProperty(IExtendedPropertyCollection owner, IExtendedPropertyDefinition propertyDefinition, Func<IList<TItem>> getDefaultValue, bool canValueBeInherited)
            : this(owner, propertyDefinition, getDefaultValue, CollectionInheritanceMode.Merge, canValueBeInherited)
        { }

        public CollectionExtendedProperty(IExtendedPropertyCollection owner, IExtendedPropertyDefinition propertyDefinition, Func<IList<TItem>> getDefaultValue, CollectionInheritanceMode inheritanceMode, bool canValueBeInherited)
            : base(owner, propertyDefinition, getDefaultValue, canValueBeInherited)
        {
            this.InheritanceMode = inheritanceMode;
        }

        protected override IList<TItem> GetActualValue()
        {
            if (IsValueSet)
            {
                if (InheritanceMode == CollectionInheritanceMode.Replace || !CanValueBeInherited)
                    return Value;
                else
                {
                    object inheritedValue = null;
                    if (Owner.TryGetInheritedPropertyValue(PropertyDefinition, out inheritedValue))
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
                                var result = (IList<TItem>)null;

                                var temp_result = new List<TItem>();
                                temp_result.AddRange((IEnumerable<TItem>)inheritedValue);
                                temp_result.AddRange((IEnumerable<TItem>)Value);

                                result = new ReadOnlyCollection<TItem>(temp_result);

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
                if (CanValueBeInherited)
                {
                    object inheritedValue = null;
                    if (Owner.TryGetInheritedPropertyValue(PropertyDefinition, out inheritedValue))
                    {
                        return (Collection<TItem>)inheritedValue;
                    }
                    else
                    {
                        return GetDefaultValue();
                    }
                }
                else
                {
                    return GetDefaultValue();
                }
            }
        }
    }
}
