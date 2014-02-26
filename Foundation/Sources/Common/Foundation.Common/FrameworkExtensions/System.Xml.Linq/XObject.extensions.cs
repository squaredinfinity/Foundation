using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace SquaredInfinity.Foundation
{
    public static class XObjectExtensions
    {
        public static string GetNameAndLocationForDebugging(this XObject obj)
        {
            var lineInfo = obj as IXmlLineInfo;

            if (obj.NodeType == XmlNodeType.Element)
                return "{0} ({1}, {2})".FormatWith(
                    (obj as XElement).Name.LocalName, 
                    lineInfo.LineNumber, 
                    lineInfo.LinePosition);

            return "({0}, {1})".FormatWith(
                lineInfo.LineNumber, 
                lineInfo.LinePosition);
        }
    }
}
