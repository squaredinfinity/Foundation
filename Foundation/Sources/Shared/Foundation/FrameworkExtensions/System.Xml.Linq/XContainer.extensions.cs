using System.Xml.Linq;
using System.Diagnostics.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Foundation.Extensions
{
    /// <summary>
    /// Extensions for XContainer calss
    /// </summary>
    public static partial class XContainerExtensions
    {
        public static IEnumerable<XNode> GetSelfAndAllDescendantNodes(this XContainer container)
        {
            yield return container;
            
            foreach (var node in container.Nodes())
            {
                if (node is XContainer)
                {
                    foreach (var childNode in (node as XContainer).GetSelfAndAllDescendantNodes())
                    {
                        yield return childNode;
                    }
                }
                else
                {
                    yield return node;
                }
            }
        }

        public static XElement FindElement(
            this XContainer container,
            IEnumerable<XElement> parentElementsTemplates,
            XElement elementTemplate)
        {
            var parentToCheck = container;

            foreach (var parentTemplate in parentElementsTemplates)
            {
                var match = parentToCheck.FindElement(parentTemplate);

                if (match == null)
                    return null;

                parentToCheck = match;
            }

            return parentToCheck.FindElement(elementTemplate);
        }

        public static XElement FindElement(
            this XContainer container,
            XElement elementTemplate)
        {
            var elementCandidates =
                from e in container.Elements()
                where object.Equals(e.Name, elementTemplate.Name)
                select e;

            foreach (var e in elementCandidates)
            {
                var attributePairs =
                    from a1 in elementTemplate.Attributes()
                    from a2 in e.Attributes()
                    where a1.Name == a2.Name
                    && a1.Value == a2.Value
                    select 1;

                if (attributePairs.Count() == elementTemplate.Attributes().Count())
                    return e;
            }

            return null;
        }

        public static XElementParentsList GetAllParents(this XContainer container)
        {
            var result = new XElementParentsList();

            if (container == null)
                return result;

            if (container.Parent == null)
                return result;

            var parent = container.Parent;

            while (parent != null)
            {
                result.Add(parent);

                parent = parent.Parent;
            }

            result.Reverse();

            return result;
        }

        public class XElementParentsList : List<XElement>
        {
            public override string ToString()
            {
                if (Count == 0)
                    return "Empty";

                StringBuilder sb = new StringBuilder();
                
                foreach (var parentElement in this)
                {
                    //sb.AppendLine(parentElement.DumpElementSignature());
                }

                return sb.ToString();
            }
        }
    }
}
