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

        IClipboardObject CreateClipboardObject();
    }

    public interface IClipboardObject
    {
        DataObject Data { get; }

        IClipboardObject Add(FrameworkElement fe);
        IClipboardObject Add(FrameworkElement fe, Size size);
        IClipboardObject AddAsHtml(FrameworkElement fe);
        IClipboardObject AddAsHtml(FrameworkElement fe, Size size);
        IClipboardObject AddAsHtml(FrameworkElement fe, Size imageSize, Size imageElementSize);

        void CopyToClipboard();
    }

    public class ClipboardObject : IClipboardObject
    {
        IClipboardService ClipboardService { get; set; }
        public DataObject Data { get; } = new DataObject();

        public ClipboardObject(IClipboardService service)
        {
            this.ClipboardService = service;
        }

        public IClipboardObject Add(FrameworkElement fe)
        {
            return Add(fe, new Size(fe.ActualWidth, fe.ActualHeight));
        }

        public IClipboardObject Add(FrameworkElement fe, Size size)
        {
            if (fe == null)
                throw new NullReferenceException(nameof(fe));

            var bmp = fe.RenderToBitmap(size);

            var mf = bmp.CreateMetafile();

            Data.SetData(DataFormats.EnhancedMetafile, mf);

            return this;
        }

        /// <summary>
        /// Copy to clipboard as HTML wiht Inline Image
        /// https://tools.ietf.org/html/rfc2397
        /// </summary>
        /// <param name="fe"></param>
        public IClipboardObject AddAsHtml(FrameworkElement fe)
        {
            var size = new Size(fe.ActualWidth, fe.ActualHeight);

            return AddAsHtml(fe, imageSize: size, imageElementSize: size);
        }

        public IClipboardObject AddAsHtml(FrameworkElement fe, Size size)
        {
            return AddAsHtml(fe, imageSize: size, imageElementSize: size);
        }

        public IClipboardObject AddAsHtml(FrameworkElement fe, Size imageSize, Size imageElementSize)
        {
            if (fe == null)
                throw new NullReferenceException(nameof(fe));

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

            var html_data = CF_HTML.PrepareHtmlFragment($"<img height=\"{imageElementSize.Height}\" width=\"{imageElementSize.Width}\" src=\"{img_src}\" />");

            Data.SetData(DataFormats.Html, html_data);

            return this;
        }

        public void CopyToClipboard()
        {
            Clipboard.Clear();
            Clipboard.SetDataObject(Data);
        }
    }

    public class ClipboardService : IClipboardService
    {
        public virtual void Clear()
        {
            Clipboard.Clear();
        }

        public IClipboardObject CreateClipboardObject()
        {
            return new ClipboardObject(this);
        }
    }
}
