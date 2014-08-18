using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Foundation.Types.Mapping
{
    public struct MappingOptions
    {
        public static readonly MappingOptions DefaultClone;
        public static readonly MappingOptions DefaultCopy;

        static MappingOptions()
        {
            DefaultClone = new MappingOptions
            {
                Mode = MappingMode.Clone,

                IgnoreNulls = false,

                ReuseTargetCollectionsWhenPossible = true,
                ReuseTargetCollectionItemsWhenPossible = true,
                TrackReferences = true,
                IgnorePrivateTypes = true
            };

            DefaultCopy = new MappingOptions
            {
                Mode = MappingMode.Copy,

                IgnoreNulls = false,

                ReuseTargetCollectionsWhenPossible = false,
                ReuseTargetCollectionItemsWhenPossible = false,
                TrackReferences = false,
                IgnorePrivateTypes = true
            };
        }

        public MappingMode Mode { get; set; }

        /// <summary>
        /// When *true*, source members with NULL value will not be mapped (original value of the target will be preserved)
        /// </summary>
        public bool IgnoreNulls { get; set; }

        public bool IgnorePrivateTypes { get; set; }

        public bool ReuseTargetCollectionsWhenPossible { get; set; }

        public bool ReuseTargetCollectionItemsWhenPossible { get; set; }

        public bool TrackReferences { get; set; }
    }

    public enum MappingMode
    {
        Clone = 0,
        Copy,
        Default = Clone
    }
}
