using System.Windows;
using System.Windows.Controls;
using LeaSearch.Plugin;
using LeaSearch.Plugin.DetailInfos;

namespace LeaSearch.TemplateSelector
{
    public class InfoContentTemplateSelector : DataTemplateSelector
    {
        public DataTemplate TextInfoDataTemplate { get; set; }

        public DataTemplate SimpleHtmlInfoDataTemplate { get; set; }

        public DataTemplate MarkDownDataTemplate { get; set; }

        public DataTemplate WebBrowserInfoDataTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null) return null;
            if (item is TextInfo)
            {
                return TextInfoDataTemplate;
            }
            if (item is MarkDownInfo)
            {
                return MarkDownDataTemplate;
            }
            if (item is WebBrowserInfo)
            {
                return WebBrowserInfoDataTemplate;
            }
            if (item is SimpleHtmlInfo)
            {
                return SimpleHtmlInfoDataTemplate;
            }
            return null;
        }

    }
}
