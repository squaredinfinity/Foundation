using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows;

namespace SquaredInfinity.Clipboard
{
    public class WindowsClipboardBuilder : IClipboardBuilder
    {
        public WindowsClipboardService Service { get; private set; }
        IClipboardService IClipboardBuilder.Service => Service;

        readonly IClipboardBuilderStep Start;
        IClipboardBuilderStep Last;

        public DataObject DataObject { get; private set; }

        public WindowsClipboardBuilder(WindowsClipboardService service)
        {
            Service = service;
            Start = BeginCopy();
            Last = Start;
        }
        
        public virtual IClipboardBuilderStep BeginCopy()
        {
            return
            new ClipboardBuilderStep(
                this,
                new BroadcastBlock<WindowsClipboardBuilder>(x => (WindowsClipboardBuilder) Service.GetClipboardBuilder()));
        }

        public virtual IClipboardBuilderStep ClearClipboard()
        {
            return
            new ClipboardBuilderStep(
                this,
                new TransformBlock<WindowsClipboardBuilder, WindowsClipboardBuilder>(x =>
                {
                    System.Windows.Clipboard.Clear();
                    return x;
                }));
        }

        public virtual IClipboardBuilderStep CopyToClipboard()
        {
            return
            new ClipboardBuilderStep(
                this,
                new TransformBlock<WindowsClipboardBuilder, WindowsClipboardBuilder>(x =>
                {
                    System.Windows.Clipboard.SetDataObject(x.DataObject);
                    return x;
                }));
        }

        public virtual IClipboardBuilderStep LinkTo(IPropagatorBlock<IClipboardBuilder, IClipboardBuilder> target)
        {
            return new ClipboardBuilderStep(this, target);
        }

        public virtual IClipboardBuilderStep SetHtml(string html)
        {
            return
            new ClipboardBuilderStep(
                this,
                new TransformBlock<WindowsClipboardBuilder, WindowsClipboardBuilder>(x =>
                {
                    DataObject.SetData(DataFormats.Html, html);
                    return x;
                }));
        }
    }
}
