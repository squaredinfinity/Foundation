using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace SquaredInfinity.Foundation.Presentation.DataTemplateSelectors
{
    public class TypeToDataTemplateMapping
    {
        public Type TargetType { get; set; }
        public string TargetTypeName { get; set; }
        public DataTemplate DataTemplate { get; set; }
    }
}
