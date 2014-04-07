using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SquaredInfinity.Foundation.Extensions
{
    public static class XElementExtensions
    {
        /// <summary>
        /// Returns a value of specified attribute.
        /// When value does not exist or does not have a value, *defaultValue* value will be returned (null by default)
        /// </summary>
        /// <param name="element">XElement</param>
        /// <param name="attributeName">name of the attribute</param>
        /// <param name="defaultValue">default value to use when attribute does not exist or does not have a value</param>
        /// <returns></returns>
        public static string GetAttributeValue(this XElement element, XName attributeName, string defaultValue = null)
        {
            var attribute = element.Attribute(attributeName);

            if (attribute == null)
                return defaultValue;

            if (attribute.Value == string.Empty)
                return defaultValue;
            else
                return attribute.Value;
        }

        /// <summary>
        /// Adds the attribute with a specified value.
        /// </summary>
        /// <param name="me">Me.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns>Newly created element</returns>
        public static XAttribute AddAttribute(this XElement me, XName name, string value)
        {
            XAttribute newAttribute = new XAttribute(name, value);
            me.Add(newAttribute);

            return newAttribute;
        }

        /// <summary>
        /// Adds the element with a specifed name.
        /// </summary>
        /// <param name="me">Me.</param>
        /// <param name="name">The name.</param>
        /// <returns>Newly created element</returns>
        public static XElement AddElement(this XElement me, XName name)
        {
            return me.AddElement(name, null);
        }

        /// <summary>
        /// Adds the element with a specified name and content.
        /// </summary>
        /// <param name="me">Me.</param>
        /// <param name="name">The name.</param>
        /// <param name="content">The content.</param>
        /// <returns>Newly created element</returns>
        public static XElement AddElement(this XElement me, XName name, object content)
        {
            XElement newElement = new XElement(name, content);
            me.Add(newElement);

            return newElement;
        }

        /// <summary>
        /// Adds the element with a specified element and content.
        /// </summary>
        /// <param name="me">Me.</param>
        /// <param name="name">The name.</param>
        /// <param name="content">The content.</param>
        /// <returns>Newly created element</returns>
        public static XElement AddElement(this XElement me, XName name, params object[] content)
        {
            XElement newElement = new XElement(name, content);
            me.Add(newElement);

            return newElement;
        }

        public static string InnerText(this XElement xElement)
        {
            StringBuilder result = new StringBuilder();

            var textNodes = from n in xElement.Nodes()
                            where n.NodeType == System.Xml.XmlNodeType.Text
                            select n as XText;

            foreach (var tn in textNodes)
            {
                result.AppendLine(tn.Value);
            }

            return result.ToString();
        }

        /// <summary>
        /// Gets Qualified Name of XElement
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static string GetQName(this XElement element)
        {
            string prefix = element.GetPrefixOfNamespace(element.Name.Namespace);

            if (element.Name.Namespace == XNamespace.None || prefix == null)
            {
                return element.Name.LocalName.ToString();
            }
            else
            {
                return prefix + ":" + element.Name.LocalName.ToString();
            }
        }

        /// <summary>
        /// Element[0] - returns element + its position
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static string NameWithPredicate(this XElement element)
        {
            if (element.Parent != null
                && element.Parent.Elements(element.Name).Count() != 1)
            {
                var qName = GetQName(element);
                var index = (element.ElementsBeforeSelf(element.Name).Count() + 1);
                return "{0}[{1}]".FormatWith(qName, index);
            }
            else
            {
                return GetQName(element);
            }
        }
    }
}
