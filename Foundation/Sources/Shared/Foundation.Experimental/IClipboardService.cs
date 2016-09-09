using System;
using SquaredInfinity.Foundation.Extensions;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using System.IO;

namespace SquaredInfinity.Foundation
{
    public interface IClipboardService
    {
        void Clear();
        void Copy(FrameworkElement fe);
        void Copy(FrameworkElement fe, Size size);
        void CopyAsHtml(FrameworkElement fe);
        void CopyAsHtml(FrameworkElement fe, Size size);
    }

    public class ClipboardService : IClipboardService
    {
        public virtual void Clear()
        {
            Clipboard.Clear();
        }

        public void Copy(FrameworkElement fe)
        {
            Copy(fe, new Size(fe.ActualWidth, fe.ActualHeight));
        }

        public virtual void Copy(FrameworkElement fe, Size size)
        {
            if (fe == null)
                throw new NullReferenceException(nameof(fe));

            var data = new DataObject();

            var bmp = fe.RenderToBitmap(size);

            var mf = bmp.CreateMetafile();
            data.SetData(DataFormats.EnhancedMetafile, mf);

            Clipboard.Clear();
            Clipboard.SetDataObject(data);
        }

        /// <summary>
        /// Copy to clipboard as HTML wiht Inline Image
        /// https://tools.ietf.org/html/rfc2397
        /// </summary>
        /// <param name="fe"></param>
        public void CopyAsHtml(FrameworkElement fe)
        {
            CopyAsHtml(fe, new Size(fe.ActualWidth, fe.ActualHeight));
        }

        public virtual void CopyAsHtml(FrameworkElement fe, Size size)
        {
            if (fe == null)
                throw new NullReferenceException(nameof(fe));

            var bmp = fe.RenderToBitmap(size);

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

            var html_data = CF_HTML.PrepareHtmlFragment($"<img height=\"{fe.ActualHeight}\" width=\"{fe.ActualWidth}\" src=\"{img_src}\" />");
                        
            var data = new DataObject();
            data.SetData(DataFormats.Html, html_data);

            Clipboard.Clear();
            Clipboard.SetDataObject(data);
        }
    }
}
