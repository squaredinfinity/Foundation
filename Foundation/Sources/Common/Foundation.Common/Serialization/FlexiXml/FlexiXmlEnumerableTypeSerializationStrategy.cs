using SquaredInfinity.Foundation.Types.Description;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using SquaredInfinity.Foundation.Extensions;

namespace SquaredInfinity.Foundation.Serialization.FlexiXml
{
    public class FlexiXmlEnumerableTypeSerializationStrategy<T, TItem> : 
        FlexiXmlTypeSerializationStrategy<T>,
        IEnumerableTypeSerializationStrategy<FlexiXmlEnumerableTypeSerializationStrategy<T,TItem>, FlexiXmlTypeSerializationStrategy<T>, T, TItem>
    {
        Predicate<TItem> ElementFilterPredicate { get; set; }

        public FlexiXmlEnumerableTypeSerializationStrategy(FlexiXmlSerializer serializer, Type type, ITypeDescriptor typeDescriptor)
            : base(serializer, type, typeDescriptor)
        {
        }

        public override void Deserialize(XElement xml, object target, IFlexiXmlSerializationContext cx)
        {
            base.Deserialize(xml, target, cx);

            var targetList = target as IList;

            if (targetList != null)
            {
                //# deserialize list elements

                var targetListDefaultElementType = targetList.GetCompatibleItemsTypes().FirstOrDefault();

                var itemElements =
                    (from el in xml.Elements()
                     where !el.IsAttached()
                     select el);

                var bulkUpdatesCollection = targetList as Collections.IBulkUpdatesCollection;

                IDisposable bulkUpdateOperation = (IDisposable)null;

                if (bulkUpdatesCollection != null)
                {
                    bulkUpdateOperation = bulkUpdatesCollection.BeginBulkUpdate();
                }

                foreach (var item_el in itemElements)
                {
                    var item = cx.Deserialize(item_el, targetListDefaultElementType, elementNameMayContainTargetTypeName: true);

                    targetList.Add(item);
                }

                if (bulkUpdateOperation != null)
                    bulkUpdateOperation.Dispose();
            }
        }

        public override XElement Serialize(object instance, IFlexiXmlSerializationContext cx, string rootElementName, out bool hasAlreadyBeenSerialized)
        {
            var el = base.Serialize(instance, cx, rootElementName, out hasAlreadyBeenSerialized);

            // if this instance has already been serialized, then there's nothing else left to do
            if (hasAlreadyBeenSerialized)
                return el;

            var list = instance as IEnumerable;

            try
            {
                // it is possible for GetEnumerator() to throw exception here
                // (e.g. NotImplemented or NotSupported)

                foreach (var item in list)
                {
                    try
                    {
                        if (!ShouldSerializeItem(item))
                            continue;

                        var el_item = cx.Serialize(item);

                        el.Add(el_item);
                    }
                    catch (Exception ex)
                    {
                        InternalTrace.Warning(ex, () => "Failed to serialize collection item.");
                    }
                }
            }
            catch (Exception ex)
            {
                InternalTrace.Warning(ex, "Failed to serialize collection items.");
            }

            return el;
        }

        protected virtual bool ShouldSerializeItem(object item)
        {
            if (ElementFilterPredicate != null)
                if (!ElementFilterPredicate((TItem)item))
                    return false;

            return true;
        }

        public FlexiXmlEnumerableTypeSerializationStrategy<T, TItem> ElementFilter(Predicate<TItem> elementFilter)
        {
            ElementFilterPredicate = elementFilter;
            return this;
        }
    }
}
