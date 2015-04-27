using SquaredInfinity.Foundation.Types.Description;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Serialization
{
    public interface IMemberSerializationStrategy
    {
        ITypeMemberDescription MemberDescription { get; }

        string MemberName { get; }

        bool CanGetValue();
        object GetValue(object memberOwner);

        bool CanSetValue();
        void SetValue(object memberOwner, object newValue);

        Func<object, bool> ShouldSerializeMember { get; set; }

        Func<object, object, string> CustomSerialize { get; set; }

        Func<string, object> CustomDeserialize { get; set; }
    }
}
