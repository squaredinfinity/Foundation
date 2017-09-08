using System;
using System.Windows.Markup;

namespace SquaredInfinity.Windows.Resources.MarkupExtensions
{
    public class LoadImageFromThisAssembly : MarkupExtension
    {
        public string RelativePath { get; set; }

        public LoadImageFromThisAssembly()
        { }

        public LoadImageFromThisAssembly(string relativePath)
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

            return ResourcesManager.LoadImage(new Uri(pack, UriKind.Absolute));
        }
    }
}
