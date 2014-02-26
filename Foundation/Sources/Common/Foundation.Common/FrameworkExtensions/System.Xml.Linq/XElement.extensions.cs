using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SquaredInfinity.Foundation
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
    }
}
