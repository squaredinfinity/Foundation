using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Foundation.Types.Description
{
    public class TypeDescription : ITypeDescription
    {
        string _assemblyQualifiedName;
        public string AssemblyQualifiedName
        {
            get { return _assemblyQualifiedName; }
            set { _assemblyQualifiedName = value; }
        }

        string _fullName;
        /// <summary>
        /// Namespace.Name
        /// </summary>
        public string FullName
        {
            get { return _fullName; }
            set { _fullName = value; }
        }

        string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }


        string _namespace;
        public string Namespace
        {
            get { return _namespace; }
            set { _namespace = value; }
        }

        readonly List<ITypeMemberDescription> _members = new List<ITypeMemberDescription>();
        public IList<ITypeMemberDescription> Members
        {
            get { return _members; }
        }
    }
}
