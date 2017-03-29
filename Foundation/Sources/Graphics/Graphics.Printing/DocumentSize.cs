using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Graphics.Printing
{
    public class DocumentSize
    {
        public PaperSize PaperSize { get; private set; }
        public MarginsSize Margins { get; private set; }
        public PaperSize AvailableArea { get; private set; }

        public DocumentSize(PaperSize paperSize, MarginsSize margins)
        {
            this.Margins = margins;
            this.PaperSize = paperSize;

            AvailableArea = 
                new PaperSize(
                    GraphicsUnits.Milimeters, 
                    paperSize.WidthMM - (margins.LeftMM + margins.RightMM), 
                    paperSize.HeightMM - (margins.TopMM - margins.BottomMM));
        }

        public DocumentSize(
            KnownPaperSizes paper, 
            Orientation orientation, 
            KnownMarginSizes margin)
        {
            var margins = new MarginsSize(margin);
            var paper_size = new PaperSize(paper);

            if (orientation == Orientation.Horizontal)
            {
                paper_size.Flip();
                margins.Flip();
            }

            Margins = margins;
            PaperSize = paper_size;

            AvailableArea =
                new PaperSize(
                    GraphicsUnits.Milimeters,
                    PaperSize.WidthMM - (Margins.LeftMM + Margins.RightMM),
                    PaperSize.HeightMM - (Margins.TopMM - Margins.BottomMM));
        }
    }
}
