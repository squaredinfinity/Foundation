using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Serialization.FlexiXmlSerialization.Services.FlexiXmlSerializationServiceTests
{
    [TestClass]
    public class SerializeTests
    {

    // .MapMember(o => o.Version); - o is for object
    //      if simple type (value type but not struct), map to attribute by default
    //      if complex type, map to element by default
    //      use member name as element / attribute name


    // .MapMember(
    ///    o => o.Version,
    ///    onConvertToXml: o => o.Version.ToString().ToXElement(); -- should return XObject
    ///    onConvertFromXml: xObj => new Version(x.Value)
    ///     
    ///    .MapMemberToElement(
    ///         "Version" | o => o.Version
    ///         convertToXml : (o, xElement) => 
    ///         {
    ///             xElement
    ///                 .AddAttribute("lol", o.P1")
    ///                 .AddAttribute("lol2", o.P2);
    ///         },
    ///         xElement =>
    ///         {
    ///         });
    ///     
    ///     
    ///     


    //    var serializationStrategy = new FlexiXmlSerializationStrategy();
    //    serializationStrategy
    //      .MapType<EmailSink>() -> TypeSerializationStrategy
    //          .ToElement("EmailSink") - elementName
    //              .MapMemberToElement(                   -> MemberSerializationStrategy
    //              .MapMemberToAttribute(
    //                  value: o => o.Subject,  
    //                  toElement : "Subject",
    //                  onConvertToXml : o => ...,
    //                  onConvertFromXml : xel => ...)
    //              .MapProperty(
    //                  property : o => o.Title,
    //                  toAttribute: "Title");
    }
}
