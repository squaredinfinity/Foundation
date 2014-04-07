using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace SquaredInfinity.Foundation.Presentation.MarkupExtensions
{
    public class LoadImageFromMainAssembly : MarkupExtension
    {
        public string RelativeUri { get; set; }
        
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Resources.LoadImageFromEntryAssembly(RelativeUri as string);
        }
    }
}
