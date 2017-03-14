using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SquaredInfinity.Presentation.DataTemplateSelectors
{
    public class ContextAwareDataTemplateSelector : DataTemplateSelector
    {
        static Func<IContextAwareDataTemplateSelectorService> _getDefaultTemplateSelectorService = () => new ContextAwareDataTemplateSelectorService();
        public static Func<IContextAwareDataTemplateSelectorService> GetDefaultTemplateSelectorService
        {
            get { return _getDefaultTemplateSelectorService; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();

                _getDefaultTemplateSelectorService = value;
            }
        }

        IContextAwareDataTemplateSelectorService _templateSelectorService;
        public IContextAwareDataTemplateSelectorService TemplateSelectorService
        {
            get
            {
                if (_templateSelectorService == null)
                    _templateSelectorService = GetDefaultTemplateSelectorService();

                return _templateSelectorService;
            }

            set
            {
                _templateSelectorService = value;
            }
        }

        public string Context { get; set; }
        public bool IsTooltip { get; set; }

        public ContextAwareDataTemplateSelector()
        { }

        public ContextAwareDataTemplateSelector(IContextAwareDataTemplateSelectorService selectorService)
        {
            this.TemplateSelectorService = selectorService;
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return TemplateSelectorService.SelectTemplate(item, container, Context, IsTooltip);
        }
    }
}
