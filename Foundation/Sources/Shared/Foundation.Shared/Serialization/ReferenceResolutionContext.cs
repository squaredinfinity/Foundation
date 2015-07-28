using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Serialization
{
    public abstract class ReferenceResolutionContext<TRoot, TypeToResolve>
    {
        public TRoot Root { get; private set; }

        public string ReferenceName { get; private set; }
        public TypeToResolve Result { get; set; }
        public bool IsSuccesful { get; set; }


        public ReferenceResolutionContext(TRoot root, string referenceName)
        {
            this.Root = root;
            this.ReferenceName = referenceName;
        }
    }
}
