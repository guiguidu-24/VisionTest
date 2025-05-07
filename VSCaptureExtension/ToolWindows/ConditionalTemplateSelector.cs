using System.Windows;
using System.Windows.Controls;

namespace VSCaptureExtension
{
    public class ConditionalTemplateSelector : DataTemplateSelector
    {
        public DataTemplate NullTemplate { get; set; }
        public DataTemplate NonNullTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return item == null ? NullTemplate : NonNullTemplate;
        }
    }
}
