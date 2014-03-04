using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.TypeReflecting
{
    public class ReflectionTypeReaderWriter : ITypeReflector
    {
        Type ReflectedType { get; set; }

        public ReflectionTypeReaderWriter(Type type)
        {
            this.ReflectedType = type;
        }

        public object GetValue(object obj, string memberName)
        {
            var member = 
                ReflectedType
                .GetProperty(memberName, BindingFlags.Instance | BindingFlags.Public);

            return member.GetValue(obj);
        }

        public void SetValue(object obj, string memberName, object newValue)
        {
            var member =
                ReflectedType
                .GetProperty(memberName, BindingFlags.Instance | BindingFlags.Public);

            member.SetValue(obj, newValue);
        }
    }
}
