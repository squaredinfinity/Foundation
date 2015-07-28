using SquaredInfinity.Foundation.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SquaredInfinity.Foundation.DotNet45.Serialization.FlexiXml
{
    public class FlexiXmlReferenceResolutionContext<TRoot, TypeToResolve> : ReferenceResolutionContext<TRoot, TypeToResolve>
    {
        public XDocument RootDocument { get; private set; }

        public FlexiXmlReferenceResolutionContext(XDocument rootDocument, TRoot root, string referenceName)
            : base(root, referenceName)
        {
            this.RootDocument = rootDocument;
        }
    }
}
