using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Graphics.Priniting
{
    public class DocumentSize
    {
        public static PaperSize A4_Portrait { get; private set; }
        public static DocumentSize A4_Portrait_NormalMargin { get; private set; }
        public static DocumentSize A4_Portrait_NarrowMargin { get; private set; }

        static DocumentSize()
        {
            A4_Portrait = new PaperSize(GraphicsUnits.Centimiters, 21.0, 29.7);

            A4_Portrait_NormalMargin = new DocumentSize(A4_Portrait, new MarginsSize(GraphicsUnits.Centimiters, 2.54, 2.54, 2.54, 2.54));
            A4_Portrait_NarrowMargin = new DocumentSize(A4_Portrait, new MarginsSize(GraphicsUnits.Centimiters, 2.54, 2.54, 2.54, 2.54));
        }

        public PaperSize Size { get; private set; }
        public MarginsSize Margins { get; private set; }
        public PaperSize AvailableArea { get; private set; }

        public DocumentSize(PaperSize paperSize, MarginsSize margins)
        {
            this.Margins = margins;
            this.Size = paperSize;
            AvailableArea = new PaperSize(GraphicsUnits.Milimeters, paperSize.WidthMM - (margins.LeftMM + margins.RightMM), paperSize.HeightMM - (margins.TopMM - margins.BottomMM));
        }
    }
}
