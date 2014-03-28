using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Serialization.FlexiXmlSerialization.Services
{
    [TestClass]
    public class FlixiXmlSerializationService__SerializeTests
    {

    /// .MapMember(o => o.Version); - o is for object
    ///      if simple type (value type but not struct), map to attribute by default
    ///      if complex type, map to element by default
    ///      use member name as element / attribute name
    /// 
    /// -- since all are opt-in, no more configuration is needed
    /// 
    /// 1. Allow serialization / deserialization on multiple threads (but how?)
    /// 
    /// s
    ///     .MapType[Configuration](MemberParticipation.OptIn)
    ///         .MapMember(
    ///             o => o.Resources) -- ResourcesCollection (.SinksCollection, .FormattersCollection)
    /// 
    /// s
    ///  .MapType[EmailSink](MemberParticipation.OptIn)
    ///  .MapMember(
    ///     o => o.BodyFormatter -- how to make it be referenced by name?
    ///     toAttribute: "bodyFormatter-ref"
    ///     onSerialize: (cx, member) => 
    ///     {
    ///         ## this all lot can probably be extracted
    ///         var root = cx.Root;
    ///         var f = member;
    ///         
    ///         using(cx.AcquireLock())
    ///         {
    ///             if(root.Resources.Formatters.ContainsKey(f.Name))
    ///             {
    ///                 if(root.Resources.Formatters[f.Name] != f)
    ///                 {
    ///                     cx.Logger.Warning(() => "formatter with given name alredy exists")
    ///                     var newName = root.Resources.Formatters.FindNextAvailableKey(f.Name);
    ///                     root.Resources.Formatters.Add(newName, f);
    ///                 }
    ///                 else
    ///                 {
    ///                     root.Resources.Formatters.Add(f.Name, f);
    ///                 }
    ///             }
    ///         
    ///             return f.Name;
    ///         },
    ///         onDeserialize: (cx, attrib) =>
    ///         {
    ///             (cx.Root as DiagnosticConfig).Resources.Formatters[attrib.Value);
    ///         });
    ///     }
    ///     
    ///  var diagConfig = new ();
    /// 
    /// service.Serialize(diagConfig, strategy);
    /// 
    /// ->
    ///     SerializationContext
    ///         obj Root = diagConfig
    ///     
    ///     
    ///     


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
