using SquaredInfinity.Foundation.Types.Description;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Serialization
{
    public interface ITypeSerializationStrategy
    {
        Version Version { get; }

        Type Type { get; }

        ITypeDescription TypeDescription { get; }
        
        void CopySerializationSetupFrom(ITypeSerializationStrategy other_strategy);

        void IgnoreMember(string memberName);
    }
}
