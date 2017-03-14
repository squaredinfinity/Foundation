using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace SquaredInfinity.Extensions
{
    public static class XObjectExtensions
    {
        public static string GetNameAndLocationForDebugging(this XObject obj)
        {
            var lineInfo = obj as IXmlLineInfo;

            if (obj.NodeType == XmlNodeType.Element)
                return $"{(obj as XElement).Name.LocalName} ({lineInfo.LineNumber}, {lineInfo.LinePosition})";

            return $"({lineInfo.LineNumber}, {lineInfo.LinePosition})";
        }

        public static TAnnotation GetOrCreateAnnotation<TAnnotation>(this XObject xobject)
            where TAnnotation : class, new()
        {
            var annotation = xobject.Annotation<TAnnotation>();

            if (annotation == null)
            {
                annotation = new TAnnotation();
                xobject.AddAnnotation(annotation);
            }

            return annotation;
        }

        public static string GetXPath(this XObject xObject)
        {
            return xObject.GetAbsoluteXPath();
        }

        /// <summary>
        /// Get the absolute XPath to a given XElement
        /// (e.g. "/people/person[6]/name[1]/last[1]").
        /// </summary>
        public static string GetAbsoluteXPath(this XObject xObject)
        {
            if (xObject == null)
            {
                throw new ArgumentNullException("element");
            }

            if (xObject is XElement)
            {
                var xElement = xObject as XElement;

                Func<XElement, string> relativeXPath = e =>
                {
                    int index = e.IndexPosition();
                    string name = e.Name.LocalName;

                    // If the element is the root, no index is required

                    return (index == -1) ? "/" + name : string.Format
                    (
                        "/{0}[{1}]",
                        name,
                        index.ToString()
                    );
                };

                var ancestors = from e in xElement.Ancestors()
                                select relativeXPath(e);

                return string.Concat(ancestors.Reverse().ToArray()) +
                       relativeXPath(xElement);
            }
            else
            {
                // todo: not supported at the moment
                return "";
            }
        }

        /// <summary>
        /// Get the index of the given XElement relative to its
        /// siblings with identical names. If the given element is
        /// the root, -1 is returned.
        /// </summary>
        /// <param name="xObject">
        /// The element to get the index of.
        /// </param>
        public static int IndexPosition(this XObject xObject)
        {
            if (xObject == null)
            {
                throw new ArgumentNullException("element");
            }

            if (xObject.Parent == null)
            {
                return -1;
            }

            int i = 1; // Indexes for nodes start at 1, not 0

            if (xObject is XAttribute)
            {
                //todo: not supported at the moment
                return 1;
            }
            else if (xObject is XElement)
            {
                var xElement = xObject as XElement;

                foreach (var sibling in xObject.Parent.Elements(xElement.Name))
                {
                    if (sibling == xObject)
                    {
                        return i;
                    }

                    i++;
                }
            }

            throw new InvalidOperationException
                ("element has been removed from its parent.");
        }

        // todo: move somewhere else
        private static string StrCat<T>(this IEnumerable<T> source,
            string separator)
        {
            return source.Aggregate(new StringBuilder(),
                       (sb, i) => sb
                           .Append(i.ToString())
                           .Append(separator),
                       s => s.ToString());
        }

    }
}
