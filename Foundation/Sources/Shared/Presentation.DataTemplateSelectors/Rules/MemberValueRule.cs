using SquaredInfinity.Foundation.Types.Description;
using SquaredInfinity.Foundation.Types.Description.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SquaredInfinity.Foundation.Extensions;

namespace SquaredInfinity.Foundation.Presentation.DataTemplateSelectors.Rules
{
    public class MemberValueRule : DataTemplateSelectorRule
    {
        public string MemberName { get; set; }
        public object ExpectedValue { get; set; }

        public bool ConvertExpectedValueToMemberType { get; set; }

        static readonly ITypeDescriptor TypeDescriptor = Types.Description.TypeDescriptor.Default;
        static readonly TypeResolver TypeResolver = TypeResolver.Default;

        public MemberValueRule()
        {
            ConvertExpectedValueToMemberType = true;
        }

        public override bool TryEvaluate(object item, System.Windows.DependencyObject container, out System.Windows.DataTemplate dt)
        {
            dt = DataTemplate;

            if (item == null)
                return false;

            var description = TypeDescriptor.DescribeType(item.GetType());

            var member = description.Members.FindMember(MemberName);

            if(member == null)
            {
                //todo: log warning
                return false;
            }

            var memberValue = member.GetValue(item);

            if (object.Equals(ExpectedValue, memberValue))
                return true;
            else
            {
                if (ExpectedValue != null && ConvertExpectedValueToMemberType)
                {
                    var memberType = member.MemberType.Type;

                    var convertedValue = ExpectedValue.Convert(memberType);

                    return object.Equals(convertedValue, memberValue);
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
