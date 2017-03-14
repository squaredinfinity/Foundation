using SquaredInfinity.Types.Description;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Serialization
{
    [DebuggerDisplay("{DebuggerDisplay}")]
    public class MemberSerializationStrategy : IMemberSerializationStrategy
    {
        public string MemberName { get; private set; }
        public ITypeMemberDescription MemberDescription { get; private set; }
        public Func<object, bool> ShouldSerializeMember { get; set; }

        public Func<object, object, string> CustomSerialize { get; set; }

        public Func<object, object> CustomDeserialize { get; set; }

        public MemberSerializationStrategy(ITypeMemberDescription memberDescription)
        {
            this.MemberDescription = memberDescription;
            this.MemberName = MemberDescription.Name;
        }

        public object GetValue(object memberOwner)
        {
            return MemberDescription.GetValue(memberOwner);
        }

        public void SetValue(object memberOwner, object newValue)
        {
            MemberDescription.SetValue(memberOwner, newValue);
        }
        public bool CanGetValue()
        {
            return MemberDescription.CanGetValue;
        }

        public bool CanSetValue()
        {
            return MemberDescription.CanSetValue;
        }

        public string DebuggerDisplay
        {
            get { return MemberName; }
        }
    }
}
