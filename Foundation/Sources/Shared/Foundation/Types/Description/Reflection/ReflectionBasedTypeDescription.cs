using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SquaredInfinity.Foundation.Extensions;

namespace SquaredInfinity.Foundation.Types.Description.Reflection
{
    public class ReflectionBasedTypeDescription : ITypeDescription
    {
        public string AssemblyQualifiedName { get; set; }

        /// <summary>
        /// Namespace.Name
        /// </summary>
        public string FullName { get; set; }

        public string Name { get; set; }

        public string Namespace { get; set; }

        ITypeMemberDescriptionCollection _members;
        public ITypeMemberDescriptionCollection Members
        {
            get { return _members; }
            set { _members = value; }
        }

        public Type Type { get; set; }


        public bool IsValueType { get; set; }

        ConstructorInfo _parameterLessConstructorInfo;
        public ConstructorInfo ParameterLessConstructorInfo 
        {
            get
            {
                if(_parameterLessConstructorInfo == null)
                {
                    _parameterLessConstructorInfo =
                        Type
                        .GetConstructor(
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                        binder: null,
                        types: Type.EmptyTypes,
                        modifiers: null);
                }

                return _parameterLessConstructorInfo;
            }
        }

        public virtual object CreateInstance()
        {
            if (ParameterLessConstructorInfo == null)
            {
                if(IsValueType)
                {
                    return Activator.CreateInstance(Type);
                }

                throw new InvalidOperationException($"Type {Type.FullName} does not have parameterless contructor");
            }

            return ParameterLessConstructorInfo.Invoke(null);
        }


        public bool AreAllMembersImmutable { get; set; }
    }
}
