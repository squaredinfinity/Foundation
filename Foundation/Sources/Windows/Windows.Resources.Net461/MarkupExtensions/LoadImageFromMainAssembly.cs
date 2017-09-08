using System;
using System.Windows.Markup;

namespace SquaredInfinity.Windows.Resources.MarkupExtensions
{
    public class LoadImageFromMainAssembly : MarkupExtension
    {
        public string RelativeUri { get; set; }
        
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return ResourcesManager.LoadImageFromEntryAssembly(RelativeUri as string);
        }
    }
}
