using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using SquaredInfinity.Foundation.Extensions;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.PropertySystem
{
    public class ExtendedPropertyCollection : IExtendedPropertyCollection
    {
        readonly Dictionary<string, IExtendedProperty> Properties = new Dictionary<string, IExtendedProperty>();

        public IExtendedPropertyContainer Owner { get; private set; }

        public bool TryGetActualPropertyValue(string uniqueName, out object value)
        {
            var prop = (IExtendedProperty)null;

            if (Properties.TryGetValue(uniqueName, out prop))
            {
                value = prop.ActualValue;
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }

        public bool TryGetInheritedPropertyValue(string uniqueName, out object inheritedValue)
        {
            //var prop = (IExtendedProperty)null;

            if (Owner.TryGetInheritedPropertyValue(uniqueName, out inheritedValue))
            {
                return true;
            }
            else
            {
                return false;
            }

            //if (Properties.TryGetValue(uniqueName, out prop))
            //{
            //    if (prop.IsValueSet)
            //    {
            //        inheritedValue = prop.Value;
            //        return true;
            //    }
            //    else
            //    {
            //        if (Owner.TryGetInheritedPropertyValue(uniqueName, out inheritedValue))
            //        {
            //            return true;
            //        }
            //        else
            //        {
            //            return false;
            //        }
            //    }
            //}
            //else
            //{
            //    inheritedValue = null;
            //    return false;
            //}
        }

        public ExtendedPropertyCollection(IExtendedPropertyContainer owner)
        {
            this.Owner = owner;
        }

        public IExtendedProperty<T> RegisterProperty<T>(string uniqueName)
        {
            var p = new ExtendedProperty<T>(this, uniqueName, () => default(T));
            Properties.Add(uniqueName, p);
            return p;
        }

        public IExtendedProperty<T> RegisterProperty<T>(string uniqueName, Func<T> getDefaultValue)
        {
            var p = new ExtendedProperty<T>(this, uniqueName, getDefaultValue);
            Properties.Add(uniqueName, p);
            return p;
        }

        public CollectionExtendedProperty<TItem> RegisterCollectionProperty<TItem>(string uniqueName, Func<Collection<TItem>> getDefaultValue)
        {
            var p = new CollectionExtendedProperty<TItem>(this, uniqueName, getDefaultValue);
            Properties.Add(uniqueName, p);
            return p;
        }


        public IExtendedProperty this[string uniqueName]
        {
            get
            {
                var prop = (IExtendedProperty)null;

                if (Properties.TryGetValue(uniqueName, out prop))
                {
                    return prop;
                }
                else
                {
                    throw new ArgumentException("Property with unique name '{0}' does not exist.".FormatWith(uniqueName));
                }
            }
        }
    }
}
