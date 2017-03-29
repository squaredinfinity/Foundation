using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SquaredInfinity.Clipboard
{
    public static class WindowsClipboardBuilderFluidInterface
    {
        public static IClipboardBuilderStep SetData(this IClipboardBuilderStep lastStep, string dataFormat, object data)
        {
            var wb = lastStep.Builder as IWindowsClipboardBuilder;

            if(wb == null)
                throw new InvalidOperationException($"{nameof(SetData)} exected {nameof(IWindowsClipboardBuilder)} builder.");

            return wb.SetData(dataFormat, data);
        }

        //public IClipboardBuilderStep AddImage(FrameworkElement fe)
        //{
        //    return Add(fe, new Size(fe.ActualWidth, fe.ActualHeight));
        //}

        //public IClipboardBuilderStep AddImage(FrameworkElement fe, Size size)
        //{
        //    if (fe == null)
        //        throw new NullReferenceException(nameof(fe));

        //    var bmp = fe.RenderToBitmap(size);

        //    var mf = bmp.CreateMetafile();

        //    Data.SetData(DataFormats.EnhancedMetafile, mf);

        //    return this;
        //}
    }
}
