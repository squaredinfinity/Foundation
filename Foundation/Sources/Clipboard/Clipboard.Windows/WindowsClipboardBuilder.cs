using System;
using SquaredInfinity.Extensions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SquaredInfinity.Clipboard.Windows
{
    public class WindowsClipboardBuilder : IClipboardBuilder
    {
        readonly List<Action<ClipboardBuilderState>> Actions = new List<Action<ClipboardBuilderState>>();

        public void Build(ClipboardBuilderState state)
        {
            foreach(var action in Actions)
            {
                action(state);
            }
        }

        public WindowsClipboardBuilder()
        {
            Actions.Add(
                x =>
                {
                    x.Properties["DataObject"] = new DataObject();
                });
        }

        #region IClipboardBuilder

        public virtual void ClearClipboard()
        {
            Actions.Add(
                x =>
                {
                    System.Windows.Clipboard.Clear();
                });
        }

        public virtual void CopyToClipboard()
        {
            Actions.Add(
                x =>
                {
                    System.Windows.Clipboard.SetDataObject(x.Properties["DataObject"] as DataObject);
                });
        }

        public virtual void SetHtml(string html)
        {
            Actions.Add(
                x =>
                {
                    (x.Properties["DataObject"] as DataObject).SetData(DataFormats.Html, html);
                });
        }

        public virtual void SetText(string text)
        {
            Actions.Add(
                x =>
                {
                    (x.Properties["DataObject"] as DataObject).SetData(DataFormats.Text, text);
                });
        }

        public virtual void Custom(Action<ClipboardBuilderState> action)
        {
            Actions.Add(action);
        }

        #endregion

        #region IWindowsClipboardBuilder

        /// <summary>
        /// Copy to clipboard as HTML wiht Inline Image
        /// https://tools.ietf.org/html/rfc2397
        /// </summary>
        /// <param name="fe"></param>
        public void AddAsHtml(FrameworkElement fe)
        {
            var size = new Size(fe.ActualWidth, fe.ActualHeight);

            AddAsHtml(fe, imageSize: size, imageElementSize: size);
        }

        public void AddAsHtml(FrameworkElement fe, Size size)
        {
            AddAsHtml(fe, imageSize: size, imageElementSize: size);
        }

        public virtual void AddAsHtml(FrameworkElement fe, Size imageSize, Size imageElementSize)
        {
            if (fe == null)
                throw new NullReferenceException(nameof(fe));

            Actions.Add(x =>
            {
                var bmp = fe.RenderToBitmap(imageSize);

                var img_src = string.Empty;

                var use_inline_image = false;
                if (use_inline_image)
                {
                    // Inline images are not actually supported by Office applications, so don't use it.
                    // maybe in future...
                    var img_data = bmp.ToBitmap().ToArray();
                    var base64_img_data = Convert.ToBase64String(img_data);
                    img_src = $"data:image/png;charset=utf-8;base64, {base64_img_data}";
                }
                else
                {
                    // create a temp file with image and use it as a source
                    var tmp = Path.GetTempFileName();
                    bmp.Save(tmp);
                    img_src = new Uri(tmp, UriKind.Absolute).ToString();
                }

                var html_data = 
                new CF_HTML()
                .PrepareHtmlFragment($"<img height=\"{imageElementSize.Height}\" width=\"{imageElementSize.Width}\" src=\"{img_src}\" />");

                (x.Properties["DataObject"] as DataObject).SetData(DataFormats.Html, html_data);
            });
        }

        #endregion
    }
}
