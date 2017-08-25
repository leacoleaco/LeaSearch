using System.Windows;
using System.Windows.Controls;
using LeaSearch.Plugin;

namespace LeaSearch.TemplateSelector
{
    public class InfoContentTemplateSelector : DataTemplateSelector
    {
        public DataTemplate TextInfoDataTemplate { get; set; }
        public DataTemplate SimpleHtmlInfoDataTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null) return null;
            if (item is TextInfo)
            {
                return TextInfoDataTemplate;
            }
            else if (item is SimpleHtmlInfo)
            {
                return SimpleHtmlInfoDataTemplate;
            }
            return null;
        }

    }
}
