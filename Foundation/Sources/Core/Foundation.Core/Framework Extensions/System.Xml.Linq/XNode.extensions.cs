using System.Xml.Linq;
using System.Diagnostics.Contracts;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Linq;
//using SquaredInfinity.Foundation.Xml.Annotations;

namespace SquaredInfinity.Foundation.Extensions
{
    /// <summary>
    /// Extensions for XNode calss
    /// </summary>
    public static partial class XNodeExtensions
    {
        public static int GetLineNumber(this XNode node)
        {
            return (node as IXmlLineInfo).LineNumber;
        }

        public static int GetLinePosition(this XNode node)
        {
            //todo: why am I subsctacting length of <!-- ??
            return (node as IXmlLineInfo).LinePosition - "<!--".Length; 
        }

        public static int GetEndLineNumber(this XNode node)
        {
            int result = node.GetLineNumber();

            switch (node.NodeType)
            {
                case XmlNodeType.Element:                   
                    if (node.NextNode != null)
                    {
                        //! for our current needs it is safe to assume that end line number = start line of next element
                        result = node.NextNode.GetLineNumber();
                    }
                    else
                    {
                        //var extendedInfoAnnotation = node.Document.Annotation<XDocumentExtendedInfoAnnotation>();

                        //if (extendedInfoAnnotation != null)
                        //{
                        //    result = extendedInfoAnnotation.DocumentLinesCount;
                        //}
                        //else
                        {
                            //! OuterXml() will remove any line breaks between attributes and therefore cannot be used as a primary source of end line information
                            result += node.OuterXml().GetLines().Count() - 1;
                        }
                    }
                    break;

                case XmlNodeType.Attribute:
                case XmlNodeType.CDATA:
                case XmlNodeType.Comment:
                case XmlNodeType.Document:
                case XmlNodeType.DocumentFragment:
                case XmlNodeType.DocumentType:
                case XmlNodeType.EndElement:
                case XmlNodeType.EndEntity:
                case XmlNodeType.Entity:
                case XmlNodeType.EntityReference:
                case XmlNodeType.None:
                case XmlNodeType.Notation:
                case XmlNodeType.ProcessingInstruction:
                case XmlNodeType.SignificantWhitespace:
                case XmlNodeType.Text:
                case XmlNodeType.Whitespace:
                case XmlNodeType.XmlDeclaration:
                default:
                    break;
            }

            return result;
        }

        /// <summary>
        /// Note, this will remove any line breaks between attributes. This is limitation of .Net (XmlReader/XmlTextReader)
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static string OuterXml(this XNode node)
        {
            var xReader = node.CreateReader();
            xReader.MoveToContent();
            return xReader.ReadOuterXml();
        }


        /// <summary>
        /// Note, this will remove any line breaks between attributes. This is limitation of .Net (XmlReader/XmlTextReader)
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static string InnerXml(this XNode node)
        {

            var xReader = node.CreateReader();
            xReader.MoveToContent();
            return xReader.ReadInnerXml();
        }
    }
}
