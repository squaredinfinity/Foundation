using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SquaredInfinity.Extensions;

namespace SquaredInfinity.Types.Description.IL
{
    public partial class ILBasedTypeDescription : ITypeDescription
    {

        string ITypeDescription.AssemblyQualifiedName
        {
            get { return Source.AssemblyQualifiedName; }
        }

        string ITypeDescription.FullName
        {
            get { return Source.FullName; }
        }

        string ITypeDescription.Name
        {
            get { return Source.Name; }
        }

        string ITypeDescription.Namespace
        {
            get { return Source.Namespace; }
        }

        ITypeMemberDescriptionCollection ITypeDescription.Members
        {
            get { return Source.Members; }
        }

        Type ITypeDescription.Type
        {
            get { return Source.Type; }
        }

        bool ITypeDescription.IsValueType
        {
            get { return Source.IsValueType; }
        }

        public object CreateInstance()
        {
            return CreateInstanceDelegate();
        }

        public bool AreAllMembersImmutable
        {
            get { return Source.AreAllMembersImmutable; }
        }
    }
}
