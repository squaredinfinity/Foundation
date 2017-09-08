using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Markup;

namespace SquaredInfinity.Windows.Resources.MarkupExtensions
{
    public class ResourceFromThisAssembly : MarkupExtension
    {
        public string RelativePath { get; set; }

        public ResourceFromThisAssembly()
        { }

        public ResourceFromThisAssembly(string relativePath)
        {
            this.RelativePath = relativePath;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var uriContext = serviceProvider.GetService(typeof(IUriContext)) as IUriContext;

            var ix_packseparator = uriContext.BaseUri.OriginalString.IndexOf(";");

            var pack = uriContext.BaseUri.OriginalString.Substring(0, ix_packseparator);

            pack += ";component";

            if (RelativePath.StartsWith(@"/"))
                pack += RelativePath;
            else
            {
                pack += @"/";
                pack += RelativePath;
            }

            return new Uri(pack);
        }
    }
}
