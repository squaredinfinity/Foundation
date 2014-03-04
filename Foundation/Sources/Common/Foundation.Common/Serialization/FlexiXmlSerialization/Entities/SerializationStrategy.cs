using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SquaredInfinity.Foundation.Serialization.FlexiXmlSerialization.Entities
{
    public class SerializationStrategy
    {        
        /// <summary>
        /// Version of serialization strategy.
        /// </summary>
        public Version Version { get; private set; }

        /// <summary>
        /// Specifies if types should participate in serialization by default.
        /// OptIn - by default types will participate in serialization (unless explicitly excluded).
        /// OptOut - by default types will NOT participate in serialization (unless explicitly included).
        /// </summary>
        public ParticipationMode TypeParticipation { get; private set; }

        /// <summary>
        /// Specifies if members should participate in serialization by default.
        /// OptIn - by default members will participate in serialization (unless explicitly excluded).
        /// OptOut - by default members will NOT participate in serialization (unless explicitly included).
        /// </summary>
        public ParticipationMode MemberParticipation { get; private set; }

        /// <summary>
        /// When MemberParticipation is set to OptIn, members to be serialized must meet Type Discovery criteria.
        /// </summary>
        public MemberDiscoveryMode MemberDiscovery { get; private set; }

        public SerializationStrategy(
            Version version,
            ParticipationMode typeParticipation = ParticipationMode.OptOut,
            ParticipationMode memberParticipation = ParticipationMode.OptOut,
            MemberDiscoveryMode memberDiscovery = MemberDiscoveryMode.Default)
        {
            this.Version = version;

            this.TypeParticipation = typeParticipation;
            this.MemberParticipation = memberParticipation;

            this.MemberDiscovery = memberDiscovery;
        }

        public TypeMappingRule MapType<T>()
        {
            var tmr = new TypeMappingRule();

            tmr.MapMember<Version>(
                "Version",
                (cx, member) =>
                {
                    return new XAttribute("Version", member);
                },
                (cx, attrib) =>
                {
                    return new Version(attrib.Value);
                });

            return null;
        }
    }
}
