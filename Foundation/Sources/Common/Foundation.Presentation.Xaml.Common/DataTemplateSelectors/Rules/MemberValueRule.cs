using SquaredInfinity.Foundation.Types.Description;
using SquaredInfinity.Foundation.Types.Description.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Presentation.DataTemplateSelectors.Rules
{
    public class MemberValueRule : DataTemplateSelectorRule
    {
        public string MemberName { get; set; }
        public object ExpectedValue { get; set; }

        static readonly ITypeDescriptor TypeDescriptor = new ReflectionBasedTypeDescriptor();

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

            if(object.Equals(ExpectedValue, member.GetValue(item)))
                return true;

            return false;
        }
    }
}
