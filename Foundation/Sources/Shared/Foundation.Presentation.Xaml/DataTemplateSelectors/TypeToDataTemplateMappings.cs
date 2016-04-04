using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Markup;

namespace SquaredInfinity.Foundation.Presentation.DataTemplateSelectors
{
    [ContentProperty("DataTemplates")]
    public class TypeToDataTemplateMappings
    {
        public TypeToDataTemplateMappingCollection DataTemplates { get; private set; }

        public TypeToDataTemplateMappings()
        {
            DataTemplates = new TypeToDataTemplateMappingCollection();
        }
    }
}
