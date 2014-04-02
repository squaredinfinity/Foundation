using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Foundation.Serialization.Mapping
{
    public class FlexiMappedInstance
    {
        public Type MappedType { get; private set; }

        public object OriginalValue { get; set; }
        public object FinalValue { get; set; }

        public FlexiMappedInstance(Type mappedType)
        {
            this.MappedType = mappedType;
        }
    }
}

//SerializationService
//.Serialize(obj, ss)


//map obj -> flexi_obj
//            .original_value
//            .final_value

//if(fv is XNode && fv == ov)
//    write cdata in element
//else
//    el.add(node)
