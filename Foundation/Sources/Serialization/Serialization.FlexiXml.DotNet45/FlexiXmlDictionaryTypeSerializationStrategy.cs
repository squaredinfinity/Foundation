using SquaredInfinity.Types.Description;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using System.Linq;
using SquaredInfinity.Extensions;

namespace SquaredInfinity.Serialization.FlexiXml
{
    public class FlexiXmlDictionaryTypeSerializationStrategy<T, TKey> : FlexiXmlTypeSerializationStrategy<T>
    {
        public class kvp
        {
            public object Key { get; set; }
            public object Value { get; set; }
        }

        Predicate<TKey> ElementFilterPredicate { get; set; }

        public FlexiXmlDictionaryTypeSerializationStrategy(FlexiXmlSerializer serializer, Type type, ITypeDescriptor typeDescriptor)
            : base(serializer, type, typeDescriptor)
        {

        }

        protected override IMemberSerializationStrategy CreateSerializationStrategyForMember(ITypeMemberDescription member)
        {
            var strategy = base.CreateSerializationStrategyForMember(member);

            if (member.Name == "Keys" || member.Name == "Values")
                strategy.ShouldSerializeMember = x => false;

            return strategy;
        }

        public override void Deserialize(XElement xml, object target, IFlexiXmlSerializationContext cx)
        {
            base.Deserialize(xml, target, cx);
            
            var targetDict = target as IDictionary;

            if (targetDict != null)
            {
                //# deserialize list elements, which should be custom kvp containers

                var itemElements =
                    (from el in xml.Elements()
                     where !el.IsAttached()
                     select el);

                var bulkUpdatesCollection = targetDict as Collections.IBulkUpdatesCollection;

                IDisposable bulkUpdateOperation = (IDisposable)null;

                if (bulkUpdatesCollection != null)
                {
                    bulkUpdateOperation = bulkUpdatesCollection.BeginBulkUpdate();
                }

                foreach (var item_el in itemElements)
                {
                    var kvp = (kvp)null;

                    if (DeserializationOutcome.Success == cx.TryDeserialize(item_el, typeof(kvp), elementNameMayContainTargetTypeName: true, deserializedInstance: out kvp))
                    {
                        targetDict[kvp.Key] = kvp.Value;
                    }
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

            var dict = instance as IDictionary;
            
            try
            {
                // it is possible for GetEnumerator() to throw exception here
                // (e.g. NotImplemented or NotSupported)

                foreach (var key in dict.Keys)
                {
                    try
                    {
                        if (!ShouldSerializeItem(key))
                            continue;

                        var kvp = new kvp
                        {
                            Key = key,
                            Value = dict[key]
                        };

                        var el_item = cx.Serialize(kvp, cx.Options.KeyValuePairElementName.ToString());

                        

                        el.Add(el_item);
                    }
                    catch (Exception ex)
                    {
                        InternalTrace.Warning(ex, () => "Failed to serialize dictionary item.");
                    }
                }
            }
            catch (Exception ex)
            {
                InternalTrace.Warning(ex, "Failed to serialize dictionary items.");
            }

            return el;
        }

        protected virtual bool ShouldSerializeItem(object item)
        {
            if (ElementFilterPredicate != null)
                if (!ElementFilterPredicate((TKey)item))
                    return false;

            return true;
        }

        public FlexiXmlDictionaryTypeSerializationStrategy<T, TKey> ElementFilter(Predicate<TKey> elementFilter)
        {
            ElementFilterPredicate = elementFilter;
            return this;
        }
    }
}
