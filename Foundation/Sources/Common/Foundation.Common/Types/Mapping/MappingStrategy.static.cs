using SquaredInfinity.Foundation.Types.Description;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Foundation.Types.Mapping
{
    public partial class MappingStrategy
    {
        public static readonly MappingStrategy Default;

        static MappingStrategy()
        {
            Default = new MappingStrategy();
            Default.CopyListElements = true;
            Default.MemberMappingStrategies.Add(new ExactNameMatchMemberMappingStrategy());
        }
    }
}
