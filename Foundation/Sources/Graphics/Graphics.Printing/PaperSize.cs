using SquaredInfinity.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Graphics.Printing
{
    public class PaperSize
    {
        public static readonly PaperSize A0 = new PaperSize(GraphicsUnits.Milimeters, 841, 1189);
        public static readonly PaperSize A1 = new PaperSize(GraphicsUnits.Milimeters, 594, 841);
        public static readonly PaperSize A2 = new PaperSize(GraphicsUnits.Milimeters, 420, 594);
        public static readonly PaperSize A3 = new PaperSize(GraphicsUnits.Milimeters, 297, 420);
        public static readonly PaperSize A4 = new PaperSize(GraphicsUnits.Milimeters, 210, 297);
        public static readonly PaperSize A5 = new PaperSize(GraphicsUnits.Milimeters, 148, 210);
        public static readonly PaperSize A6 = new PaperSize(GraphicsUnits.Milimeters, 105, 148);
        public static readonly PaperSize A7 = new PaperSize(GraphicsUnits.Milimeters, 74, 105);
        public static readonly PaperSize A8 = new PaperSize(GraphicsUnits.Milimeters, 52, 74);
        public static readonly PaperSize A9 = new PaperSize(GraphicsUnits.Milimeters, 37, 52);
        public static readonly PaperSize A10 = new PaperSize(GraphicsUnits.Milimeters, 26, 37);

        public static readonly PaperSize B0 = new PaperSize(GraphicsUnits.Milimeters, 1000, 1414);
        public static readonly PaperSize B1 = new PaperSize(GraphicsUnits.Milimeters, 707, 1000);
        public static readonly PaperSize B2 = new PaperSize(GraphicsUnits.Milimeters, 500, 707);
        public static readonly PaperSize B3 = new PaperSize(GraphicsUnits.Milimeters, 353, 500);
        public static readonly PaperSize B4 = new PaperSize(GraphicsUnits.Milimeters, 250, 353);
        public static readonly PaperSize B5 = new PaperSize(GraphicsUnits.Milimeters, 176, 250);
        public static readonly PaperSize B6 = new PaperSize(GraphicsUnits.Milimeters, 125, 176);
        public static readonly PaperSize B7 = new PaperSize(GraphicsUnits.Milimeters, 88, 125);
        public static readonly PaperSize B8 = new PaperSize(GraphicsUnits.Milimeters, 62, 88);
        public static readonly PaperSize B9 = new PaperSize(GraphicsUnits.Milimeters, 44, 62);
        public static readonly PaperSize B10 = new PaperSize(GraphicsUnits.Milimeters, 31, 44);

        public static readonly PaperSize C0 = new PaperSize(GraphicsUnits.Milimeters, 917, 1297);
        public static readonly PaperSize C1 = new PaperSize(GraphicsUnits.Milimeters, 648, 917);
        public static readonly PaperSize C2 = new PaperSize(GraphicsUnits.Milimeters, 458, 648);
        public static readonly PaperSize C3 = new PaperSize(GraphicsUnits.Milimeters, 324, 458);
        public static readonly PaperSize C4 = new PaperSize(GraphicsUnits.Milimeters, 229, 324);
        public static readonly PaperSize C5 = new PaperSize(GraphicsUnits.Milimeters, 162, 229);
        public static readonly PaperSize C6 = new PaperSize(GraphicsUnits.Milimeters, 114, 162);
        public static readonly PaperSize C7 = new PaperSize(GraphicsUnits.Milimeters, 81, 114);
        public static readonly PaperSize C8 = new PaperSize(GraphicsUnits.Milimeters, 57, 81);
        public static readonly PaperSize C9 = new PaperSize(GraphicsUnits.Milimeters, 40, 57);
        public static readonly PaperSize C10 = new PaperSize(GraphicsUnits.Milimeters, 28, 40);

        public double WidthMM { get; private set; }
        public double HeightMM { get; private set; }

        public PaperSize(GraphicsUnits units, double width, double height)
        {
            if (units == GraphicsUnits.Centimiters)
            {
                WidthMM = width * 10;
                HeightMM = height * 10;
            }
            else if (units == GraphicsUnits.Milimeters)
            {
                WidthMM = width;
                HeightMM = height;
            }
            else
            {
                throw new NotSupportedException(nameof(units));
            }
        }

        public PaperSize(KnownPaperSizes size)
        {
            switch (size)
            {
                case KnownPaperSizes.A0:
                    CopyFrom(A0);
                    break;
                case KnownPaperSizes.A1:
                    CopyFrom(A1);
                    break;
                case KnownPaperSizes.A2:
                    CopyFrom(A2);
                    break;
                case KnownPaperSizes.A3:
                    CopyFrom(A3);
                    break;
                case KnownPaperSizes.A4:
                    CopyFrom(A4);
                    break;
                case KnownPaperSizes.A5:
                    CopyFrom(A5);
                    break;
                case KnownPaperSizes.A6:
                    CopyFrom(A6);
                    break;
                case KnownPaperSizes.A7:
                    CopyFrom(A7);
                    break;
                case KnownPaperSizes.A8:
                    CopyFrom(A8);
                    break;
                case KnownPaperSizes.A9:
                    CopyFrom(A9);
                    break;
                case KnownPaperSizes.A10:
                    CopyFrom(A10);
                    break;
                case KnownPaperSizes.B0:
                    CopyFrom(B0);
                    break;
                case KnownPaperSizes.B1:
                    CopyFrom(B1);
                    break;
                case KnownPaperSizes.B2:
                    CopyFrom(B2);
                    break;
                case KnownPaperSizes.B3:
                    CopyFrom(B3);
                    break;
                case KnownPaperSizes.B4:
                    CopyFrom(B4);
                    break;
                case KnownPaperSizes.B5:
                    CopyFrom(B5);
                    break;
                case KnownPaperSizes.B6:
                    CopyFrom(B6);
                    break;
                case KnownPaperSizes.B7:
                    CopyFrom(B7);
                    break;
                case KnownPaperSizes.B8:
                    CopyFrom(B8);
                    break;
                case KnownPaperSizes.B9:
                    CopyFrom(B9);
                    break;
                case KnownPaperSizes.B10:
                    CopyFrom(B10);
                    break;
                case KnownPaperSizes.C0:
                    CopyFrom(C0);
                    break;
                case KnownPaperSizes.C1:
                    CopyFrom(C1);
                    break;
                case KnownPaperSizes.C2:
                    CopyFrom(C2);
                    break;
                case KnownPaperSizes.C3:
                    CopyFrom(C3);
                    break;
                case KnownPaperSizes.C4:
                    CopyFrom(C4);
                    break;
                case KnownPaperSizes.C5:
                    CopyFrom(C5);
                    break;
                case KnownPaperSizes.C6:
                    CopyFrom(C6);
                    break;
                case KnownPaperSizes.C7:
                    CopyFrom(C7);
                    break;
                case KnownPaperSizes.C8:
                    CopyFrom(C8);
                    break;
                case KnownPaperSizes.C9:
                    CopyFrom(C9);
                    break;
                case KnownPaperSizes.C10:
                    CopyFrom(C10);
                    break;
                default:
                    throw new NotSupportedException($"{size}");
            }
        }

        public void Flip()
        {
            var height = HeightMM;
            HeightMM = WidthMM;
            WidthMM = height;
        }

        public void CopyFrom(PaperSize other)
        {
            this.WidthMM = other.WidthMM;
            this.HeightMM = other.HeightMM;
        }
    }
}
