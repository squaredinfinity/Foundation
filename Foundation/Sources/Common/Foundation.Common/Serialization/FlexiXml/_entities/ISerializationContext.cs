using SquaredInfinity.Foundation.Types.Description;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SquaredInfinity.Foundation.Serialization.FlexiXml
{
    /// <summary>
    /// Serialization Context holds state information of current serialization / deserialization operation
    /// </summary>
    public interface ISerializationContext
    {
        /// <summary>
        /// Type
        /// </summary>
        ITypeDescriptor TypeDescriptor { get; }

        /// <summary>
        /// Instance of root object being serialized
        /// </summary>
        object RootInstance { get; }

        /// <summary>
        /// Root xml element of xml document being serialized / deserialized
        /// </summary>
        XElement RootElement { get; }

        // todo: this should not be exposed in that way
        ConcurrentDictionary<object, InstanceId> Objects_InstanceIdTracker { get; }

        // todo: this should not be exposed in that way
        ConcurrentDictionary<InstanceId, object> Objects_ById { get; }

        /// <summary>
        /// Serialization Options
        /// </summary>
        SerializationOptions Options { get; }

        object CreateInstance(Type type, CreateInstanceContext cx);

        XElement Serialize(object item);
        XElement Serialize(object memberValue, string rootElementName);
        
        /// <summary>
        /// Deserialize xml to specified target type.
        /// Note that xml may provide additional deserialization information which will result in instance of different type being returned.
        /// Target Type Candidate will always be assignable from resulting instance.
        /// </summary>
        /// <param name="xml">xml containing serialization information</param>
        /// <param name="targetTypeCandidate">minimum type to which xml will be deserialized (min base type)</param>
        /// <returns>deserialized instance</returns>
        object Deserialize(XElement xml, Type targetTypeCandidate, bool elementNameMayContainTargetTypeName);

        /// <summary>
        /// Deserialize xml to specified object instance.
        /// Members of specified target will be populated using data from xml.
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="target"></param>
        /// <param name="cx"></param>
        void Deserialize(XElement xml, object target);

        ITypeSerializationStrategy GetTypeSerializationStrategy(Type type);

        long GetNextUniqueId();

        TypeResolver TypeResolver { get; }
    }
}
