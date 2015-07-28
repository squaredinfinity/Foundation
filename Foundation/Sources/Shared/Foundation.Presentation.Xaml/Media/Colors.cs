using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SquaredInfinity.Foundation.Presentation.Media
{
    public class Colors
    {
        static Func<IEnumerable<Color>> GetPredefinedColors { get; set; }

        public static void SetPredefinedColorsProvider(Func<IEnumerable<Color>> getPredefinedColors)
        {
            GetPredefinedColors = getPredefinedColors;
        }


        static IReadOnlyList<Color> _predefined = null;
        
        public static IReadOnlyList<Color> Predefined
        {
            get
            {
                if (_predefined == null)
                {
                    _predefined = GetPredefinedColors().ToArray();
                }

                return _predefined;
            }
        }

        static Colors()
        {
            GetPredefinedColors = GetDefaultPredefinedColors;
        }

        static IEnumerable<Color> GetDefaultPredefinedColors()
        {
            var colorType = typeof(System.Windows.Media.Color);

            foreach (var p in typeof(System.Windows.Media.Colors).GetProperties())
            {
                if (p.PropertyType == colorType)
                    yield return (Color)p.GetValue(obj: null);
            }
        }

        public static IReadOnlyList<Color> GetPredefined()
        {
            return Predefined;
        }
    }
}
