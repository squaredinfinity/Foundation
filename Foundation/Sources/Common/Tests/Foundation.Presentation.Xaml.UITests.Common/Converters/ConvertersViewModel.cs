using SquaredInfinity.Foundation.Presentation.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Foundation.Presentation.Xaml.UITests.Converters
{
    public class ConvertersViewModel : ViewModel
    {
        List<Color> _colors;
        public List<Color> Colors
        {
            get { return _colors; }
        }

        List<bool> _booleanValues;
        public List<bool> BooleanValues
        {
            get { return _booleanValues; }
        }

        public ConvertersViewModel()
        {
            _colors = new List<Color>()
            {
                Color.FromRgb(0xB0, 0x00, 0x00),
                Color.FromRgb(0x00, 0x63, 0x00),
                Color.FromRgb(0x00, 0x00, 0xBE),
                Color.FromRgb(0xCD, 0x00, 0x6A)
            };

            _booleanValues = new List<bool>()
            {
                true,
                false
            };

            
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            Window w;

            TryFindDataContextInVisualTree<Window>(out w);
        }

        protected override void OnAfterDataContextChanged(object newDataContext)
        {
            base.OnAfterDataContextChanged(newDataContext);
        }
    }
}
