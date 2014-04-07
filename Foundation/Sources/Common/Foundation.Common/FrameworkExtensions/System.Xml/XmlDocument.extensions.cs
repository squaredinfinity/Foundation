using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace SquaredInfinity.Foundation.Extensions
{
    public static class XmlDocumentExtensions
    {
        public static XDocument ToXDocument(this XmlDocument me)
        {
            using (var nodeReader = new XmlNodeReader(me))
            {
                nodeReader.MoveToContent();
                return XDocument.Load(nodeReader);
            }
        }
    }
}
