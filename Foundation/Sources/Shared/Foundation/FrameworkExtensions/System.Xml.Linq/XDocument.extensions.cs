using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.IO;
//using SquaredInfinity.Foundation.Xml.Annotations;

namespace SquaredInfinity.Foundation.Extensions
{
    public static class XDocumentExtensions
    {
        public static XmlDocument ToXmlDocument(this XDocument me)
        {
            var xmlDocument = new XmlDocument();

            using (var xmlReader = me.CreateReader())
            {
                xmlDocument.Load(xmlReader);
            }

            return xmlDocument;
        }

        public static bool TryParse(string xmlString, LoadOptions loadOptions, out XDocument parsedDocument)
        {
            parsedDocument = null;

            try
            {

                // ! XDocument.Parse will flatten attributes and replace any tabs|new lines with spaces
                // ! this happens even when LoadOptions.PreserveWhitespace is set
                // ! code below works around this issue by setting WhitespaceHadnling directly on XmlReader

                using (StringReader stringReader = new StringReader(xmlString))
                {
                    using (XmlTextReader xmlReader = new XmlTextReader(stringReader))
                    {
                        xmlReader.WhitespaceHandling = WhitespaceHandling.All;

                        xmlReader.DtdProcessing = DtdProcessing.Ignore;

                        // ! overloaded method below will process all other LoadOptions
                        parsedDocument = XDocument.Load(xmlReader, loadOptions);

                        //var extendedInfoAnnotation = new XDocumentExtendedInfoAnnotation(xmlString.GetLines().Length);

                        //parsedDocument.AddAnnotation(extendedInfoAnnotation);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ex.TryAddContextData("xml string", () => xmlString);
                InternalTrace.Information(ex, "failed parsing xml document");
                return false;
            }
        }
    }
}
