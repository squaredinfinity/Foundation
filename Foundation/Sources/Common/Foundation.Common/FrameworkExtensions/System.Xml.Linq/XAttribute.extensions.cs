using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace SquaredInfinity.Foundation.Extensions
{
    public static class XAttributeExtensions
    {
        // todo: can we achieve the same without regex by using method from GetQName?
        public static string PrefixedName(this XAttribute me)
        {
            if(me.IsNamespaceDeclaration)
            {
                if (me.Name.LocalName.Equals("xmlns", StringComparison.InvariantCultureIgnoreCase))
                    return "xmlns";

                return "xmlns:{0}".FormatWith(me.Name.LocalName);
            }

            var match = Regex.Match(me.ToString(), "^(?<namespacePrefix>[^:^ ^\"]+):.+");
            if (match.Success)
            {
                return "{0}:{1}".FormatWith(match.Groups["namespacePrefix"].Value, me.Name.LocalName);
            }
            
            return me.Name.LocalName;
        }

        public static string GetQName(this XAttribute attribute)
        {
            string prefix = attribute.Parent.GetPrefixOfNamespace(attribute.Name.Namespace);

            if (attribute.Name.Namespace == XNamespace.None || prefix == null)
            {
                return attribute.Name.ToString();
            }
            else
            {
                return prefix + ":" + attribute.Name.LocalName;
            }
        }

    }
}
