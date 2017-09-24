using System.Windows;
using System.Windows.Controls;
using LeaSearch.Plugin.DetailInfos;

namespace LeaSearch.TemplateSelector
{
    public class InfoContentTemplateSelector : DataTemplateSelector
    {
        public DataTemplate TextInfoDataTemplate { get; set; }

        public DataTemplate SimpleHtmlInfoDataTemplate { get; set; }

        public DataTemplate MarkDownDataTemplate { get; set; }

        public DataTemplate FlowDocumentInfoDataTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null) return null;
            if (item is TextInfo)
            {
                return TextInfoDataTemplate;
            }
            if (item is FlowDocumentInfo)
            {
                return FlowDocumentInfoDataTemplate;
            }
            if (item is MarkDownInfo)
            {
                return MarkDownDataTemplate;
            }
            if (item is SimpleHtmlInfo)
            {
                return SimpleHtmlInfoDataTemplate;
            }
            return null;
        }

    }
}
